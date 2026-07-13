using Events.Models.Entities;

namespace Events.Services.Interfaces;

public interface ISiteContentService
{
    Task<SiteContent> GetAsync();
    Task UpdateHeroAsync(string heroTitleBg, string heroTitleEn, string heroSubtitleBg, string heroSubtitleEn);

    // Sanitizes both bodies (bold, links, paragraphs only) before persisting.
    Task UpdateAboutUsAsync(string aboutUsContentBg, string aboutUsContentEn);
}
