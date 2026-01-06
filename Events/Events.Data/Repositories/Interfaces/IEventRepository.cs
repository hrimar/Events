using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null);
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Event>> GetByLocationAsync(string location);
    Task<IEnumerable<Event>> SearchAsync(string searchTerm);
    Task<Event> AddAsync(Event eventEntity);
    Task<Event> UpdateAsync(Event eventEntity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    // pagination and filtering
    Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
    int page,
    int pageSize,
    EventStatus? status = null,
    string? categoryName = null,
    bool? isFree = null,
    DateTime? fromDate = null);

    Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10);
    Task<int> GetTotalEventsCountAsync(EventStatus? status = null);
}