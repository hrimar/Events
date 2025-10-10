using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Event>> GetBySubCategoryIdAsync(int subCategoryId);
    Task<IEnumerable<Event>> GetByLocationAsync(string location);
    Task<IEnumerable<Event>> SearchAsync(string searchTerm);
    Task<Event> AddAsync(Event eventEntity);
    Task<Event> UpdateAsync(Event eventEntity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}