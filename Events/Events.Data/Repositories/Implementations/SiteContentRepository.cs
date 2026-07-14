using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class SiteContentRepository : ISiteContentRepository
{
    private readonly EventsDbContext _context;

    public SiteContentRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<SiteContent> GetAsync()
    {
        var siteContent = await _context.SiteContents.FirstOrDefaultAsync();
        if (siteContent != null)
            return siteContent;

        // Defensive fallback in case the seed row was ever removed or missed.
        siteContent = new SiteContent { UpdatedAt = DateTime.UtcNow };
        _context.SiteContents.Add(siteContent);
        await _context.SaveChangesAsync();
        return siteContent;
    }

    public async Task<SiteContent> UpdateAsync(SiteContent siteContent)
    {
        _context.SiteContents.Update(siteContent);
        await _context.SaveChangesAsync();
        return siteContent;
    }
}
