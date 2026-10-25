using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface IHomeQuickFilterRepository
{
    // All filters (active and inactive), by SortOrder then Id. Tracked, so the service can change and save them.
    Task<List<HomeQuickFilter>> GetAllAsync();

    // Active filters only, by SortOrder then Id. Read-only (homepage).
    Task<List<HomeQuickFilter>> GetActiveAsync();

    Task<HomeQuickFilter?> GetByIdAsync(int id);
    Task AddAsync(HomeQuickFilter filter);
    Task DeleteAsync(HomeQuickFilter filter);

    // Persists changes made to filters loaded through this repository.
    Task SaveChangesAsync();
}
