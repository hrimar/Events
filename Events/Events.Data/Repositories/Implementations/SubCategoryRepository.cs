using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly EventsDbContext _context;

    public SubCategoryRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<SubCategory?> GetByIdAsync(int id)
    {
        return await _context.SubCategories
            .Include(sc => sc.Events)
            .FirstOrDefaultAsync(sc => sc.Id == id);
    }

    public async Task<IEnumerable<SubCategory>> GetAllAsync()
    {
        return await _context.SubCategories
            .OrderBy(sc => sc.ParentCategory)
            .ThenBy(sc => sc.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<SubCategory>> GetByCategoryAsync(EventCategory category)
    {
        return await _context.SubCategories
            .Where(sc => sc.ParentCategory == category)
            .OrderBy(sc => sc.Name)
            .ToListAsync();
    }

    public async Task<SubCategory?> GetByEnumValueAsync(EventCategory category, int enumValue)
    {
        return await _context.SubCategories
            .FirstOrDefaultAsync(sc => sc.ParentCategory == category && sc.EnumValue == enumValue);
    }

    public async Task<SubCategory?> GetByNameAsync(EventCategory category, string name)
    {
        return await _context.SubCategories
            .FirstOrDefaultAsync(sc => sc.ParentCategory == category &&
                                      sc.Name.ToLower() == name.ToLower());
    }

    public async Task<SubCategory> AddAsync(SubCategory subCategory)
    {
        subCategory.CreatedAt = DateTime.UtcNow;

        _context.SubCategories.Add(subCategory);
        await _context.SaveChangesAsync();
        return subCategory;
    }

    public async Task<SubCategory> UpdateAsync(SubCategory subCategory)
    {
        _context.SubCategories.Update(subCategory);
        await _context.SaveChangesAsync();
        return subCategory;
    }

    public async Task DeleteAsync(int id)
    {
        var subCategory = await _context.SubCategories.FindAsync(id);
        if (subCategory != null)
        {
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.SubCategories.AnyAsync(sc => sc.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(EventCategory category, string name)
    {
        return await _context.SubCategories
            .AnyAsync(sc => sc.ParentCategory == category &&
                           sc.Name.ToLower() == name.ToLower());
    }

    public async Task<int> GetCountByCategoryAsync(EventCategory category)
    {
        return await _context.SubCategories
            .CountAsync(sc => sc.ParentCategory == category);
    }

    public async Task<IEnumerable<SubCategory>> GetSubCategoriesWithEventsAsync(EventCategory category)
    {
        return await _context.SubCategories
            .Include(sc => sc.Events.Where(e => e.Status == EventStatus.Published))
            .Where(sc => sc.ParentCategory == category && sc.Events.Any())
            .OrderBy(sc => sc.Name)
            .ToListAsync();
    }
}