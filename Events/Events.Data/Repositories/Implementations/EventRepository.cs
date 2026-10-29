using System;
using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class EventRepository : IEventRepository
{
    private readonly EventsDbContext _context;

    public EventRepository(EventsDbContext context)
    {
        _context = context;
    }

    // Intentionally NOT AsNoTracking(): TagService.RemoveTagFromEventAsync loads an Event via this
    // method, removes an item from its EventTags collection, then calls UpdateAsync - it relies on
    // EF change tracking to detect the collection removal and delete the EventTag row. AsNoTracking()
    // here would silently stop tag removal from working (no error, the tag just stays associated).
    public async Task<Event?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null)
    {
        var query = _context.Events.AsQueryable();
        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        // Use exclusive upper bound (< next day) so events at any time during toDate are included.
        // Guard against DateTime.MaxValue overflow before calling AddDays(1).
        var exclusiveEnd = toDate.Date == DateTime.MaxValue.Date ? DateTime.MaxValue : toDate.Date.AddDays(1);
        query = query.Where(e => e.Date >= fromDate.Date && e.Date < exclusiveEnd);
        return await query.CountAsync();
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(PublicEventQuery eventQuery, CancellationToken cancellationToken = default)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .AsQueryable();
        //var queryString = query.ToQueryString(); // TODO: Use this generated SQL and analize it in via MSSMS Actual Execution Plan!!!

        query = ApplyPublicFilters(query, eventQuery);

        var totalCount = await query.CountAsync(cancellationToken);

        var orderedQuery = ApplySorting(query, eventQuery.SortBy, eventQuery.SortOrder);

        var events = await orderedQuery
            .Skip((eventQuery.Page - 1) * eventQuery.PageSize)
            .Take(eventQuery.PageSize)
            .ToListAsync(cancellationToken);

        return (events, totalCount);
    }

    public async Task<List<EventListingKeyDto>> GetListingKeysAsync(PublicEventQuery eventQuery, int maxCount,
        CancellationToken cancellationToken = default)
    {
        return await ApplyPublicFilters(_context.Events.AsNoTracking(), eventQuery)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Id)
            .Take(maxCount)
            .Select(e => new EventListingKeyDto(e.Id, e.Date, e.StartTime, e.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Event>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return new List<Event>();
        }

        var idList = ids.Distinct().ToList();
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => idList.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }

    // Status, category/subcategory, free, date range, tags and search filters shared by the public listings.
    private static IQueryable<Event> ApplyPublicFilters(IQueryable<Event> query, PublicEventQuery eventQuery)
    {
        if (eventQuery.Status.HasValue)
        {
            var status = eventQuery.Status.Value;
            query = query.Where(e => e.Status == status);
        }

        // Priority: subCategoryName > categoryNames
        var subCategoryName = eventQuery.SubCategoryName;
        var categoryNames = eventQuery.CategoryNames.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        if (!string.IsNullOrEmpty(subCategoryName))
        {
            query = query.Where(e =>
                e.SubCategory != null &&
                e.SubCategory.Name == subCategoryName);
        }
        else if (categoryNames.Count == 1)
        {
            // Single category keeps the plain equality predicate (same SQL as before).
            var categoryName = categoryNames[0];
            query = query.Where(e =>
                e.Category != null &&
                e.Category.Name == categoryName);
        }
        else if (categoryNames.Count > 1)
        {
            query = query.Where(e =>
                e.Category != null &&
                categoryNames.Contains(e.Category.Name));
        }

        if (eventQuery.IsFree.HasValue)
        {
            var isFree = eventQuery.IsFree.Value;
            query = query.Where(e => e.IsFree == isFree);
        }

        if (eventQuery.FromDate.HasValue)
        {
            var fromDate = eventQuery.FromDate.Value;
            query = query.Where(e => e.Date >= fromDate);
        }

        // Use exclusive upper bound (< next day) so events at any time during toDate are included.
        if (eventQuery.ToDate.HasValue)
        {
            var toDate = eventQuery.ToDate.Value;
            var exclusiveEnd = toDate.Date == DateTime.MaxValue.Date
                ? DateTime.MaxValue
                : toDate.Date.AddDays(1);
            query = query.Where(e => e.Date < exclusiveEnd);
        }

        var tagList = eventQuery.TagNames.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        if (tagList.Count > 0)
        {
            query = query.Where(e => e.EventTags.Any(et => et.Tag != null && tagList.Contains(et.Tag.Name)));
        }

        var requiredTagList = eventQuery.RequiredTagNames.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        if (requiredTagList.Count > 0)
        {
            query = query.Where(e => e.EventTags.Any(et => et.Tag != null && requiredTagList.Contains(et.Tag.Name)));
        }

        if (!string.IsNullOrWhiteSpace(eventQuery.Search))
        {
            query = ApplySearch(query, eventQuery.Search);
        }

        return query;
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsAsync(EventListCriteria criteria)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .AsQueryable();

        if (criteria.Status.HasValue)
        {
            query = query.Where(e => e.Status == criteria.Status.Value);
        }

        if (criteria.SubCategoryId.HasValue)
        {
            query = query.Where(e => e.SubCategoryId == criteria.SubCategoryId.Value);
        }
        else if (criteria.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.IsFree.HasValue)
        {
            query = query.Where(e => e.IsFree == criteria.IsFree.Value);
        }

        if (criteria.FromDate.HasValue)
        {
            query = query.Where(e => e.Date >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            var exclusiveEnd = criteria.ToDate.Value.Date == DateTime.MaxValue.Date
                ? DateTime.MaxValue
                : criteria.ToDate.Value.AddDays(1);
            query = query.Where(e => e.Date < exclusiveEnd);
        }

        if (criteria.CreatedAtFrom.HasValue)
        {
            query = query.Where(e => e.CreatedAt >= criteria.CreatedAtFrom.Value);
        }

        if (criteria.CreatedAtTo.HasValue)
        {
            var exclusiveEnd = criteria.CreatedAtTo.Value.Date == DateTime.MaxValue.Date
                ? DateTime.MaxValue
                : criteria.CreatedAtTo.Value.AddDays(1);
            query = query.Where(e => e.CreatedAt < exclusiveEnd);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            query = ApplySearch(query, criteria.Search);
        }

        var totalCount = await query.CountAsync();
        var orderedQuery = ApplySorting(query, criteria.SortBy, criteria.SortOrder);

        var events = await orderedQuery
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (events, totalCount);
    }

    // Single definition of what "matches the search term" means for every listing.
    // Case-insensitivity (incl. Cyrillic) comes from the database collation on nvarchar columns.
    private static IQueryable<Event> ApplySearch(IQueryable<Event> query, string searchTerm)
    {
        return query.Where(e =>
            e.Name.Contains(searchTerm) ||
            (e.Description != null && e.Description.Contains(searchTerm)) ||
            e.Location.Contains(searchTerm));
    }

    private static IQueryable<Event> ApplySorting(IQueryable<Event> query, string? sortBy, string sortOrder)
    {
        var normalizedSort = (sortBy ?? "date").ToLowerInvariant();
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return normalizedSort switch
        {
            "name" => isDescending
                ? query.OrderByDescending(e => e.Name)
                : query.OrderBy(e => e.Name),
            "time" => isDescending
                ? query.OrderByDescending(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero)
                : query.OrderBy(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero),
            "category" => isDescending
                ? query.OrderByDescending(e => e.Category != null ? e.Category.Name : string.Empty)
                : query.OrderBy(e => e.Category != null ? e.Category.Name : string.Empty),
            "subcategory" => isDescending
                ? query.OrderByDescending(e => e.SubCategory != null ? e.SubCategory.Name : string.Empty)
                : query.OrderBy(e => e.SubCategory != null ? e.SubCategory.Name : string.Empty),
            "location" => isDescending
                ? query.OrderByDescending(e => e.Location)
                : query.OrderBy(e => e.Location),
            "status" => isDescending
                ? query.OrderByDescending(e => e.Status)
                : query.OrderBy(e => e.Status),
            "price" => isDescending
                ? query.OrderByDescending(e => e.IsFree ? 0m : (e.Price.HasValue ? e.Price.Value : decimal.MaxValue))
                : query.OrderBy(e => e.IsFree ? 0m : (e.Price.HasValue ? e.Price.Value : decimal.MaxValue)),
            "featured" => isDescending
                ? query.OrderByDescending(e => e.IsFeatured)
                : query.OrderBy(e => e.IsFeatured),
            "createdat" => isDescending
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt),
            _ => isDescending
                ? query.OrderByDescending(e => e.Date).ThenByDescending(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero)
                : query.OrderBy(e => e.Date).ThenBy(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero)
        };
    }

    public async Task<IEnumerable<Event>> GetFeaturedEventsAsync(DateTime fromDate, int count = 10)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            // fromDate is the calendar day (Europe/Sofia "today" from the caller), not a full timestamp:
            // comparing against "now" would drop today's events hours before their actual start time.
            .Where(e => e.IsFeatured && e.Status == EventStatus.Published && e.Date >= fromDate.Date)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.StartTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<CategoryUpcomingSummaryDto>> GetTopUpcomingCategoriesAsync(
        DateTime fromDate, DateTime toDate, int count, string excludedImageUrl)
    {
        var from = fromDate.Date;
        var exclusiveEnd = toDate.Date.AddDays(1);

        // Counted in SQL (GROUP BY), only the top categories come back.
        var topCounts = await _context.Events
            .AsNoTracking()
            .Where(e => e.Status == EventStatus.Published && e.Date >= from && e.Date < exclusiveEnd)
            .Where(e => e.Category.CategoryType != EventCategory.Undefined)
            .GroupBy(e => e.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(count)
            .ToListAsync();

        var categoryIds = topCounts.Select(c => c.CategoryId).ToList();

        // One row per category; the nearest event image comes from a correlated subquery (no events are loaded).
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.CategoryType,
                c.DefaultImageUrl,
                NearestEventImageUrl = c.Events
                    .Where(e => e.Status == EventStatus.Published && e.Date >= from
                        && e.ImageUrl != null && e.ImageUrl != "" && e.ImageUrl != excludedImageUrl)
                    .OrderBy(e => e.Date)
                    .ThenBy(e => e.StartTime)
                    .Select(e => e.ImageUrl)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(c => c.Id);

        return topCounts
            .Where(c => categories.ContainsKey(c.CategoryId))
            .Select(c =>
            {
                var category = categories[c.CategoryId];
                return new CategoryUpcomingSummaryDto
                {
                    CategoryType = category.CategoryType,
                    Name = category.Name,
                    UpcomingEventsCount = c.Count,
                    DefaultImageUrl = category.DefaultImageUrl,
                    NearestEventImageUrl = category.NearestEventImageUrl
                };
            })
            .ToList();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(DateTime fromDate, int count = 10)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            // Calendar day, not a full timestamp - see GetFeaturedEventsAsync above for why.
            .Where(e => e.Status == EventStatus.Published && e.Date >= fromDate.Date)
            .OrderBy(e => e.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetTotalEventsCountAsync(EventStatus? status = null)
    {
        var query = _context.Events.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        return await query.CountAsync();
    }

    // Without .ToLower() as the DB collation (SQL_Latin1_General_CP1_CI_AS) is already case-insensitive.
    // .ToLower() wrapped the column, so no index on Name could be used.
    public async Task<Event?> FindByNameAsync(string name)
    {
        return await _context.Events.AsNoTracking().Where(e => e.Name == name).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> FindAllByNameAsync(string name)
    {
        return await _context.Events.AsNoTracking().Where(e => e.Name == name).ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByCategoryAsync(EventCategory category)
    {
        var categoryId = (int)category;

        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.CategoryId == categoryId)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByLocationAsync(string location)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.Location.Contains(location))
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var query = _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag);

        return await ApplySearch(query, searchTerm)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event> AddAsync(Event eventEntity)
    {
        eventEntity.CreatedAt = DateTime.UtcNow;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();
        return eventEntity;
    }

    public async Task<Event> UpdateAsync(Event eventEntity)
    {
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _context.Events.Update(eventEntity);
        await _context.SaveChangesAsync();
        return eventEntity;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Events.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.CategoryId == categoryId)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity != null)
        {
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Batch update multiple events with a single database transaction.
    /// All events are marked for update, then SaveChanges is called ONCE for all of them.
    /// 
    /// Performance benefits:
    /// - Single SaveChanges call (not N calls)
    /// - Single database transaction (all or nothing)
    /// - Significantly faster than sequential updates
    /// - Optimal for bulk admin operations
    /// </summary>
    public async Task<int> BulkUpdateAsync(IEnumerable<Event> events)
    {
        try
        {
            if (events == null || !events.Any())
            {
                return 0;
            }

            var eventList = events.ToList();

            // Mark all events for update (doesn't hit database yet)
            foreach (var eventEntity in eventList)
            {
                eventEntity.UpdatedAt = DateTime.UtcNow;
                _context.Events.Update(eventEntity);
            }

            // Single efficient SaveChanges call for ALL events
            await _context.SaveChangesAsync();

            return eventList.Count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during bulk update of events", ex);
        }
    }

    public async Task<int> UpdateCanonicalVenueByLocationAsync(string location, int canonicalVenueId)
    {
        try
        {
            var events = await _context.Events
                .Where(e => e.Location == location)
                .ToListAsync();

            foreach (var ev in events)
            {
                ev.CanonicalVenueId = canonicalVenueId;
                ev.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return events.Count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating canonical venue for location '{location}'", ex);
        }
    }
}