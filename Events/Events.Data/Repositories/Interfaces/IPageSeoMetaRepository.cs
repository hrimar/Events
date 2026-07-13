using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface IPageSeoMetaRepository
{
    Task<IEnumerable<PageSeoMeta>> GetAllAsync();
    Task<PageSeoMeta?> GetByKeyAsync(string pageKey);

    // Updates an existing set of rows in one transaction (admin "SEO" tab saves all rows at once).
    Task UpdateManyAsync(IEnumerable<PageSeoMeta> pages);
}
