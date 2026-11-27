using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.Data.SqlClient;
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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2627 or 2601 })
        {
            // Another worker saved the same text between the check above and this insert (GS-REQ-010, 10.4). The entry is already
            // in the cache, so this is not an error. Detached, so the next SaveChanges of this context does not try to insert it again.
            _context.Entry(entry).State = EntityState.Detached;
        }
    }
}
