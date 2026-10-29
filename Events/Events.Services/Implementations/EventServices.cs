using Events.Services.Caching;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Events.Services.Implementations;

public class EventService : IEventService
{
    // Long TTL is safe because entries also get invalidated immediately on any write
    // (see _cacheInvalidator.Invalidate() below) - this is just a safety net, not the
    // primary freshness mechanism. Events only change via the daily crawler run or
    // occasional admin edits, so a few minutes of staleness (if invalidation were ever
    // missed) would be harmless anyway.
    private static readonly TimeSpan PagedEventsCacheDuration = TimeSpan.FromMinutes(5);

    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;
    private readonly IMemoryCache _cache;
    private readonly IEventCacheInvalidator _cacheInvalidator;
    private readonly ISofiaClock _clock;

    public EventService(
        IEventRepository eventRepository,
        ILogger<EventService> logger,
        IMemoryCache cache,
        IEventCacheInvalidator cacheInvalidator,
        ISofiaClock clock)
    {
        _eventRepository = eventRepository;
        _logger = logger;
        _cache = cache;
        _cacheInvalidator = cacheInvalidator;
        _clock = clock;
    }

    public async Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _eventRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event with ID {EventId}", id);
            throw new ApplicationException($"Failed to retrieve event with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        try
        {
            return await _eventRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all events");
            throw new ApplicationException("Failed to retrieve events", ex);
        }
    }

    public async Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null)
    {
        try
        {
            return await _eventRepository.GetEventsCountInRangeAsync(fromDate, toDate, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events count in range {FromDate} - {ToDate}", fromDate, toDate);
            throw new ApplicationException($"Failed to get events count in range {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}", ex);
        }
    }

    public async Task<Event?> FindEventByNameAsync(string name)
    {
        try
        {
            return await _eventRepository.FindByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding event by name: {Name}", name);
            return null;
        }
    }

    public async Task<IEnumerable<Event>> FindEventsByNameAsync(string name)
    {
        try
        {
            return await _eventRepository.FindAllByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding events by name: {Name}", name);
            return [];
        }
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(PublicEventQuery eventQuery,
        CancellationToken cancellationToken = default) // Not passed to the cached DB call below - see the
                                                       // CancellationToken.None comment where that query runs. Used only for uncached search queries.
    {
        try
        {
            eventQuery = Normalize(eventQuery);

            _logger.LogInformation(
                "Getting paged events: Page {Page}, PageSize {PageSize}, Status {Status}, Category {Category}," +
                "SubCategory {SubCategory}, FromDate {FromDate}, SortBy {SortBy}, SortOrder {SortOrder}, Search {Search}",
                eventQuery.Page, eventQuery.PageSize, eventQuery.Status, string.Join(",", eventQuery.CategoryNames), eventQuery.SubCategoryName,
                eventQuery.FromDate, eventQuery.SortBy, eventQuery.SortOrder, eventQuery.Search);

            // Search terms are free user input (autocomplete fires on every keystroke) - caching them would
            // fill the unbounded IMemoryCache with one-off entries, so search queries always go to the database.
            if (eventQuery.Search != null)
            {
                return await _eventRepository.GetPagedEventsAsync(eventQuery, cancellationToken);
            }

            return await GetCachedAsync(BuildPagedEventsCacheKey(eventQuery),
                () => _eventRepository.GetPagedEventsAsync(eventQuery, CancellationToken.None));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged events");
            throw new ApplicationException("Failed to retrieve paged events", ex);
        }
    }

    public async Task<List<EventListingKeyDto>> GetListingKeysAsync(PublicEventQuery eventQuery, int maxCount,
        CancellationToken cancellationToken = default)
    {
        try
        {
            eventQuery = Normalize(eventQuery);

            // Search terms are not cached, for the same reason as in GetPagedEventsAsync.
            if (eventQuery.Search != null)
            {
                return await _eventRepository.GetListingKeysAsync(eventQuery, maxCount, cancellationToken);
            }

            var cacheKey = string.Join("|", "ListingKeys", maxCount, BuildPagedEventsCacheKey(eventQuery with { Page = 1, PageSize = 0 }));
            return await GetCachedAsync(cacheKey,
                () => _eventRepository.GetListingKeysAsync(eventQuery, maxCount, CancellationToken.None));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event listing keys");
            throw new ApplicationException("Failed to retrieve event listing keys", ex);
        }
    }

    public async Task<List<Event>> GetEventsByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ids.Count == 0)
            {
                return new List<Event>();
            }

            // Sorted, so the same set of ids in a different order still hits the same cache entry.
            var cacheKey = "EventsByIds|" + string.Join(",", ids.Distinct().OrderBy(id => id));
            return await GetCachedAsync(cacheKey, () => _eventRepository.GetByIdsAsync(ids, CancellationToken.None));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by ids");
            throw new ApplicationException("Failed to retrieve events by ids", ex);
        }
    }

    // Caches a public listing result for PagedEventsCacheDuration; any event write invalidates it.
    private Task<T> GetCachedAsync<T>(string cacheKey, Func<Task<T>> fetch)
    {
        // The cached value is a Lazy<Task<...>>, not the result itself: IMemoryCache.GetOrCreate
        // only guarantees the factory delegate runs once, but concurrent callers could still each
        // start their own DB call before the first one finishes writing to the cache. Wrapping the
        // fetch in a Lazy makes concurrent callers for the same key await the SAME in-flight task
        // instead of each issuing their own query (single-flight / anti "thundering herd").
        var lazyResult = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SetAbsoluteExpiration(PagedEventsCacheDuration);
            entry.AddExpirationToken(new CancellationChangeToken(_cacheInvalidator.Token));

            // CancellationToken.None, not the caller's own token (callers pass it in fetch): this fetch
            // is shared across whichever concurrent requests happen to hit the same cache key. If it used
            // the first caller's token, that caller disconnecting would cancel the query for every
            // other caller waiting on the same Lazy<Task<...>> too, even ones still connected.
            return new Lazy<Task<T>>(fetch);
        });

        return lazyResult!.Value;
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsAsync(EventListCriteria criteria)
    {
        try
        {
            _logger.LogInformation(
                "Getting filtered events: Page {Page}, PageSize {PageSize}, Status {Status}, " +
                "CategoryId {CategoryId}, SubCategoryId {SubCategoryId}, Search {Search}, " +
                "FromDate {FromDate}, ToDate {ToDate}, SortBy {SortBy}, SortOrder {SortOrder}",
                criteria.Page, criteria.PageSize, criteria.Status, criteria.CategoryId, criteria.SubCategoryId,
                criteria.Search, criteria.FromDate, criteria.ToDate, criteria.SortBy, criteria.SortOrder);

            return await _eventRepository.GetFilteredEventsAsync(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered events");
            throw new ApplicationException("Failed to retrieve filtered events", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10)
    {
        try
        {
            count = Math.Min(count, 50);
            return await _eventRepository.GetFeaturedEventsAsync(_clock.Today, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured events");
            throw new ApplicationException("Failed to retrieve featured events", ex);
        }
    }

    public async Task<List<CategoryUpcomingSummaryDto>> GetTopUpcomingCategoriesAsync(int days, int count, string excludedImageUrl)
    {
        try
        {
            var today = _clock.Today;
            return await _eventRepository.GetTopUpcomingCategoriesAsync(today, today.AddDays(days - 1), count, excludedImageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top upcoming categories");
            throw new ApplicationException("Failed to retrieve top upcoming categories", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10)
    {
        try
        {
            count = Math.Min(count, 100);
            return await _eventRepository.GetUpcomingEventsAsync(_clock.Today, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming events");
            throw new ApplicationException("Failed to retrieve upcoming events", ex);
        }
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Event>();
            }

            searchTerm = searchTerm.Trim();

            return await _eventRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events with term {SearchTerm}", searchTerm);
            throw new ApplicationException($"Failed to search events with term '{searchTerm}'", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category)
    {
        try
        {
            return await _eventRepository.GetByCategoryAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by category {Category}", category);
            throw new ApplicationException($"Failed to retrieve events for category {category}", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be after end date");
            }

            return await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by date range {StartDate} - {EndDate}", startDate, endDate);
            throw new ApplicationException($"Failed to retrieve events for date range {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", ex);
        }
    }

    public async Task<Event> CreateEventAsync(Event eventEntity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eventEntity.Name))
            {
                throw new ArgumentException("Event name is required");
            }

            var created = await _eventRepository.AddAsync(eventEntity);
            _cacheInvalidator.Invalidate();
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event {EventName}", eventEntity.Name);
            throw new ApplicationException($"Failed to create event '{eventEntity.Name}'", ex);
        }
    }

    public async Task<Event> UpdateEventAsync(Event eventEntity)
    {
        try
        {
            if (!await _eventRepository.ExistsAsync(eventEntity.Id))
            {
                throw new InvalidOperationException($"Event with ID {eventEntity.Id} not found");
            }

            var updated = await _eventRepository.UpdateAsync(eventEntity);
            _cacheInvalidator.Invalidate();
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", eventEntity.Id);
            throw new ApplicationException($"Failed to update event with ID {eventEntity.Id}", ex);
        }
    }

    public async Task DeleteEventAsync(int id)
    {
        try
        {
            if (!await _eventRepository.ExistsAsync(id))
            {
                throw new InvalidOperationException($"Event with ID {id} not found");
            }

            await _eventRepository.DeleteAsync(id);
            _cacheInvalidator.Invalidate();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw new ApplicationException($"Failed to delete event with ID {id}", ex);
        }
    }

    public async Task<int> GetTotalEventsCountAsync(EventStatus? status = null)
    {
        try
        {
            return await _eventRepository.GetTotalEventsCountAsync(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total events count");
            throw new ApplicationException("Failed to get events count", ex);
        }
    }

    public async Task<bool> EventExistsAsync(int id)
    {
        try
        {
            return await _eventRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if event exists {EventId}", id);
            return false;
        }
    }

    public async Task<int> BulkUpdateEventsAsync(IEnumerable<Event> events)
    {
        try
        {
            if (events == null || !events.Any())
            {
                return 0;
            }

            // Batch update with single transaction:
            // Mark all events for update and call SaveChanges once.
            // This is significantly faster than sequential individual updates.
            int updatedCount = await _eventRepository.BulkUpdateAsync(events);

            _cacheInvalidator.Invalidate();

            _logger.LogInformation("Bulk update completed: {UpdatedCount} events updated in single transaction", updatedCount);

            return updatedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk update of events");
            throw new ApplicationException("Failed to bulk update events", ex);
        }
    }

    // Clamps paging and trims the search term (blank search becomes null).
    private static PublicEventQuery Normalize(PublicEventQuery eventQuery)
    {
        var pageSize = eventQuery.PageSize;
        // Allow large pageSize for "get all" scenarios (sitemap uses int.MaxValue)
        if (pageSize < 1) pageSize = 12;
        if (pageSize > 50000) pageSize = 50000; // Safety limit to prevent memory issues

        return eventQuery with
        {
            Page = eventQuery.Page < 1 ? 1 : eventQuery.Page,
            PageSize = pageSize,
            Search = string.IsNullOrWhiteSpace(eventQuery.Search) ? null : eventQuery.Search.Trim()
        };
    }

    private static string BuildPagedEventsCacheKey(PublicEventQuery eventQuery)
    {
        // Sorted so the same set of tags (or categories) in a different order still hits the same cache entry.
        var sortedTags = string.Join(",", eventQuery.TagNames.OrderBy(t => t, StringComparer.OrdinalIgnoreCase));
        var sortedCategories = string.Join(",", eventQuery.CategoryNames.OrderBy(c => c, StringComparer.OrdinalIgnoreCase));

        return string.Join("|", "PagedEvents", eventQuery.Page, eventQuery.PageSize, eventQuery.Status,
            sortedCategories, eventQuery.SubCategoryName, eventQuery.IsFree, eventQuery.FromDate?.ToString("O"),
            eventQuery.SortBy, eventQuery.SortOrder, eventQuery.ToDate?.ToString("O"), sortedTags);
    }
}