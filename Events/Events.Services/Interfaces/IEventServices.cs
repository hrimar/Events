using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null);
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);

    Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        EventStatus? status = null,
        string? categoryName = null,
        string? subCategoryName = null,
        bool? isFree = null,
        DateTime? fromDate = null,
        string? sortBy = null,
        string sortOrder = "asc");

    Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<int> GetTotalEventsCountAsync(EventStatus? status = null);
    Task<bool> EventExistsAsync(int id);

    /// <summary>
    /// Batch update multiple events efficiently in a single database transaction.
    /// 
    /// Advantages of batch operations:
    /// - Single database round-trip (not N round-trips for N events)
    /// - One transaction scope ensures data consistency
    /// - Optimal for admin operations affecting 5-100 events
    /// - Better performance than sequential individual updates
    /// </summary>
    Task<int> BulkUpdateEventsAsync(IEnumerable<Event> events);
}