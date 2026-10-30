using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class HomeQuickFilterRepository : IHomeQuickFilterRepository
{
    private readonly EventsDbContext _context;

    public HomeQuickFilterRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<List<HomeQuickFilter>> GetAllAsync()
    {
        return await _context.HomeQuickFilters
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Id)
            .ToListAsync();
    }

    public async Task<List<HomeQuickFilter>> GetActiveAsync()
    {
        return await _context.HomeQuickFilters
            .AsNoTracking()
            .Where(f => f.IsActive)
            .OrderBy(f => f.SortOrder)
            .ThenBy(f => f.Id)
            .ToListAsync();
    }

    public async Task<HomeQuickFilter?> GetByIdAsync(int id)
    {
        return await _context.HomeQuickFilters.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task AddAsync(HomeQuickFilter filter)
    {
        _context.HomeQuickFilters.Add(filter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(HomeQuickFilter filter)
    {
        _context.HomeQuickFilters.Remove(filter);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
