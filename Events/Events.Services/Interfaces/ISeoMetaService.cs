using Events.Models.Entities;

namespace Events.Services.Interfaces;

public interface ISeoMetaService
{
    Task<IEnumerable<PageSeoMeta>> GetAllAsync();

    // Returns the row for the given page key, or null if none exists (callers should fall back gracefully).
    Task<PageSeoMeta?> GetByKeyAsync(string pageKey);

    // Saves all rows at once (admin "SEO" tab submits every page together).
    Task SaveAllAsync(IEnumerable<PageSeoMeta> pages);
}
