using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

// Stored machine translations (GS-REQ-010, 10.4).
public interface ITranslationCacheRepository
{
    // The cached translation for the hash, or null. Read-only.
    Task<TranslationCacheEntry?> GetAsync(string hash);

    // Saves the entry; does nothing when an entry with the same hash already exists.
    Task AddAsync(TranslationCacheEntry entry);
}
