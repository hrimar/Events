using Events.Services.Caching;
using Events.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
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

    public EventService(
        IEventRepository eventRepository,
        ILogger<EventService> logger,
        IMemoryCache cache,
        IEventCacheInvalidator cacheInvalidator)
    {
        _eventRepository = eventRepository;
        _logger = logger;
        _cache = cache;
        _cacheInvalidator = cacheInvalidator;
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

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        EventStatus? status = null,
        string? categoryName = null,
        string? subCategoryName = null,
        bool? isFree = null,
        DateTime? fromDate = null,
        string? sortBy = null,
        string sortOrder = "asc",
        DateTime? toDate = null,
        IEnumerable<string>? tagNames = null,
        CancellationToken cancellationToken = default) // Accepted for interface consistency but intentionally not passed
        // to the DB call below - see the CancellationToken.None comment where the query actually runs.
    {
        try
        {
            if (page < 1) page = 1;

            // Allow large pageSize for "get all" scenarios (Controller uses int.MaxValue)
            if (pageSize < 1) pageSize = 12;
            if (pageSize > 50000) pageSize = 50000; // Safety limit to prevent memory issues

            _logger.LogInformation(
                "Getting paged events: Page {Page}, PageSize {PageSize}, Status {Status}, Category {Category}," +
                "SubCategory {SubCategory}, FromDate {FromDate}, SortBy {SortBy}, SortOrder {SortOrder}",
                page, pageSize, status, categoryName, subCategoryName, fromDate, sortBy, sortOrder);

            var cacheKey = BuildPagedEventsCacheKey(
                page, pageSize, status, categoryName, subCategoryName, isFree, fromDate, sortBy, sortOrder, toDate, tagNames);

            // The cached value is a Lazy<Task<...>>, not the result itself: IMemoryCache.GetOrCreate
            // only guarantees the factory delegate runs once, but concurrent callers could still each
            // start their own DB call before the first one finishes writing to the cache. Wrapping the
            // fetch in a Lazy makes concurrent callers for the same key await the SAME in-flight task
            // instead of each issuing their own query (single-flight / anti "thundering herd").
            var lazyResult = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(PagedEventsCacheDuration);
                entry.AddExpirationToken(new CancellationChangeToken(_cacheInvalidator.Token));

                // CancellationToken.None, not the caller's own token: this fetch is shared across
                // whichever concurrent requests happen to hit the same cache key. If it used the
                // first caller's token, that caller disconnecting would cancel the query for every
                // other caller waiting on the same Lazy<Task<...>> too, even ones still connected.
                return new Lazy<Task<(IEnumerable<Event> Events, int TotalCount)>>(() => _eventRepository.GetPagedEventsAsync(
                        page, pageSize, status, categoryName, subCategoryName, isFree, fromDate, sortBy, sortOrder, toDate, tagNames, CancellationToken.None));
            });

            return await lazyResult!.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged events");
            throw new ApplicationException("Failed to retrieve paged events", ex);
        }
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
            return await _eventRepository.GetFeaturedEventsAsync(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured events");
            throw new ApplicationException("Failed to retrieve featured events", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10)
    {
        try
        {
            count = Math.Min(count, 100);
            return await _eventRepository.GetUpcomingEventsAsync(count);
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

    private static string BuildPagedEventsCacheKey(
        int page,
        int pageSize,
        EventStatus? status,
        string? categoryName,
        string? subCategoryName,
        bool? isFree,
        DateTime? fromDate,
        string? sortBy,
        string sortOrder,
        DateTime? toDate,
        IEnumerable<string>? tagNames)
    {
        // Sorted so the same set of tags in a different order still hits the same cache entry.
        var sortedTags = tagNames is null
            ? string.Empty
            : string.Join(",", tagNames.OrderBy(t => t, StringComparer.OrdinalIgnoreCase));

        return string.Join("|", "PagedEvents", page, pageSize, status, categoryName, subCategoryName, isFree,
            fromDate?.ToString("O"), sortBy, sortOrder, toDate?.ToString("O"), sortedTags);
    }
}