using Events.Models.Entities;
using Events.Services.Models.Admin;

namespace Events.Services.Interfaces;

// Admin management of the homepage "What" chips (GS-REQ-007, 7.3): add, edit, reorder, enable/disable.
public interface IHomeQuickFilterService
{
    // All filters for the admin list, in display order.
    Task<List<HomeQuickFilter>> GetAllAsync();

    // Active filters for the homepage, in display order (at most HomeQuickFilter.MaxActive).
    Task<List<HomeQuickFilter>> GetActiveAsync();

    Task<HomeQuickFilter?> GetByIdAsync(int id);

    // New filters are added at the end of the list.
    Task<HomeQuickFilterResult> CreateAsync(HomeQuickFilterInput input);
    Task<HomeQuickFilterResult> UpdateAsync(int id, HomeQuickFilterInput input);
    Task<HomeQuickFilterResult> DeleteAsync(int id);
    Task<HomeQuickFilterResult> SetActiveAsync(int id, bool isActive);

    // Swaps the filter with its neighbour; offset -1 moves it up, +1 down.
    Task<HomeQuickFilterResult> MoveAsync(int id, int offset);
}
