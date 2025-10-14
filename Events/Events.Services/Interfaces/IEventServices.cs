using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);

    Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        EventStatus? status = null,
        string? categoryName = null,
        bool? isFree = null,
        DateTime? fromDate = null);

    Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<int> GetTotalEventsCountAsync(EventStatus? status = null);
    Task<bool> EventExistsAsync(int id);
}