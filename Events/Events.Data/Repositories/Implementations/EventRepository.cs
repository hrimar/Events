using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
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

    public async Task DeleteAsync(int id)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity != null)
        {
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }
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

    public async Task<IEnumerable<Event>> GetBySubCategoryIdAsync(int subCategoryId)
    {
        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.SubCategoryId == subCategoryId)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }
}