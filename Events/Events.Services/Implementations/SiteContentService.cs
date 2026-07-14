using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Helpers;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class SiteContentService : ISiteContentService
{
    private readonly ISiteContentRepository _repository;
    private readonly ILogger<SiteContentService> _logger;

    public SiteContentService(ISiteContentRepository repository, ILogger<SiteContentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SiteContent> GetAsync() => await _repository.GetAsync();

    public async Task UpdateHeroAsync(string heroTitleBg, string heroTitleEn, string heroSubtitleBg, string heroSubtitleEn)
    {
        var siteContent = await _repository.GetAsync();

        siteContent.HeroTitleBg = heroTitleBg;
        siteContent.HeroTitleEn = heroTitleEn;
        siteContent.HeroSubtitleBg = heroSubtitleBg;
        siteContent.HeroSubtitleEn = heroSubtitleEn;
        siteContent.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(siteContent);
        _logger.LogInformation("Hero content updated");
    }

    public async Task UpdateAboutUsAsync(string aboutUsContentBg, string aboutUsContentEn)
    {
        var siteContent = await _repository.GetAsync();

        siteContent.AboutUsContentBg = RichTextSanitizer.Sanitize(aboutUsContentBg);
        siteContent.AboutUsContentEn = RichTextSanitizer.Sanitize(aboutUsContentEn);
        siteContent.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(siteContent);
        _logger.LogInformation("About Us content updated");
    }
}
