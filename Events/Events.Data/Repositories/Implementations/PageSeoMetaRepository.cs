using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class PageSeoMetaRepository : IPageSeoMetaRepository
{
    private readonly EventsDbContext _context;

    public PageSeoMetaRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PageSeoMeta>> GetAllAsync()
    {
        return await _context.PageSeoMetas
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<PageSeoMeta?> GetByKeyAsync(string pageKey)
    {
        return await _context.PageSeoMetas.FirstOrDefaultAsync(p => p.PageKey == pageKey);
    }

    public async Task UpdateManyAsync(IEnumerable<PageSeoMeta> pages)
    {
        // Matched by PageKey (stable, unique) rather than trusting the incoming Id from the form.
        var pageKeys = pages.Select(p => p.PageKey).ToList();
        var existing = await _context.PageSeoMetas
            .Where(p => pageKeys.Contains(p.PageKey))
            .ToDictionaryAsync(p => p.PageKey);

        foreach (var page in pages)
        {
            if (!existing.TryGetValue(page.PageKey, out var entity))
                continue;

            entity.TitleBg = page.TitleBg;
            entity.TitleEn = page.TitleEn;
            entity.DescriptionBg = page.DescriptionBg;
            entity.DescriptionEn = page.DescriptionEn;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
