using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetByCategoryAsync(Events.Models.Enums.EventCategory category);
    Task<IEnumerable<Event>> GetByLocationAsync(string location);
    Task<IEnumerable<Event>> SearchAsync(string searchTerm);
    Task<Event> AddAsync(Event eventEntity);
    Task<Event> UpdateAsync(Event eventEntity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}