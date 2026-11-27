using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class TranslationCacheRepository : ITranslationCacheRepository
{
    private readonly EventsDbContext _context;

    public TranslationCacheRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<TranslationCacheEntry?> GetAsync(string hash)
    {
        return await _context.TranslationCacheEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Hash == hash);
    }

    public async Task AddAsync(TranslationCacheEntry entry)
    {
        if (await _context.TranslationCacheEntries.AnyAsync(t => t.Hash == entry.Hash))
            return;

        if (entry.CreatedAt == default)
            entry.CreatedAt = DateTime.UtcNow;

        _context.TranslationCacheEntries.Add(entry);
        await _context.SaveChangesAsync();
    }
}
