using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface ISiteContentRepository
{
    // Returns the singleton SiteContent row, creating a default one if missing (e.g. seed was skipped).
    Task<SiteContent> GetAsync();
    Task<SiteContent> UpdateAsync(SiteContent siteContent);
}
