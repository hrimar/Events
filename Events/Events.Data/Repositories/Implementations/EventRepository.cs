using System;
using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
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

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events
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

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        EventStatus? status = null,
        string? categoryName = null,
        string? subCategoryName = null,
        bool? isFree = null,
        DateTime? fromDate = null,
        string? sortBy = null,
        string sortOrder = "asc")
    {
        var query = _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .AsQueryable();
        //var queryString = query.ToQueryString(); // TODO: Use this generated SQL and analize it in via MSSMS Actual Execution Plan!!!

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        // Priority: subCategoryName > categoryName
        if (!string.IsNullOrEmpty(subCategoryName))
        {
            query = query.Where(e =>
                e.SubCategory != null &&
                e.SubCategory.Name == subCategoryName);
        }
        else if (!string.IsNullOrEmpty(categoryName))
        {
            query = query.Where(e =>
                e.Category != null &&
                e.Category.Name == categoryName);
        }

        if (isFree.HasValue)
        {
            query = query.Where(e => e.IsFree == isFree.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(e => e.Date >= fromDate.Value);
        }

        var totalCount = await query.CountAsync();

        var orderedQuery = ApplySorting(query, sortBy, sortOrder);

        var events = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (events, totalCount);
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsAsync(EventListCriteria criteria)
    {
        var query = _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
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

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var searchTerm = criteria.Search;
            query = query.Where(e =>
                e.Name.Contains(searchTerm) ||
                (e.Description != null && e.Description.Contains(searchTerm)) ||
                e.Location.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        var orderedQuery = ApplySorting(query, criteria.SortBy, criteria.SortOrder);

        var events = await orderedQuery
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (events, totalCount);
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
            _ => isDescending
                ? query.OrderByDescending(e => e.Date).ThenByDescending(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero)
                : query.OrderBy(e => e.Date).ThenBy(e => e.StartTime.HasValue ? e.StartTime.Value : TimeSpan.Zero)
        };
    }

    public async Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.IsFeatured && e.Status == EventStatus.Published && e.Date >= DateTime.UtcNow)
            .OrderBy(e => e.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.Status == EventStatus.Published && e.Date >= DateTime.UtcNow)
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

    public async Task<Event?> FindByNameAsync(string name)
    {
        return await _context.Events.Where(e => e.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
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
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.Location.Contains(location))
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> SearchAsync(string searchTerm)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.Name.Contains(searchTerm) ||
                       e.Description!.Contains(searchTerm) ||
                       e.Location.Contains(searchTerm))
            .OrderBy(e => e.Date)
            .ToListAsync();
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
}