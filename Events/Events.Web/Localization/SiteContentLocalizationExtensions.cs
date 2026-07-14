using Events.Models.Entities;

namespace Events.Web.Localization;

// Picks the BG or EN field of SiteContent/PageSeoMeta based on the current UI culture.
// Lives in Events.Web for the same reason as CategoryLocalizationExtensions: keeps Events.Models free
// of presentation concerns.
public static class SiteContentLocalizationExtensions
{
    public static string LocalizedHeroTitle(this SiteContent content, bool isEnglish) =>
        isEnglish ? content.HeroTitleEn : content.HeroTitleBg;

    public static string LocalizedHeroSubtitle(this SiteContent content, bool isEnglish) =>
        isEnglish ? content.HeroSubtitleEn : content.HeroSubtitleBg;

    public static string LocalizedAboutUsContent(this SiteContent content, bool isEnglish) =>
        isEnglish ? content.AboutUsContentEn : content.AboutUsContentBg;

    public static string? LocalizedTitle(this PageSeoMeta meta, bool isEnglish) =>
        isEnglish ? meta.TitleEn : meta.TitleBg;

    public static string? LocalizedDescription(this PageSeoMeta meta, bool isEnglish) =>
        isEnglish ? meta.DescriptionEn : meta.DescriptionBg;
}
