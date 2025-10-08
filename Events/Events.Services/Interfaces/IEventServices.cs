using Events.Models.Entities;

namespace Events.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetEventsByCategoryAsync(Events.Models.Enums.EventCategory category);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);
}