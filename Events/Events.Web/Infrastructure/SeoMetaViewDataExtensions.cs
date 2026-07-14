using Events.Models.Entities;
using Events.Web.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Events.Web.Infrastructure;

public static class SeoMetaViewDataExtensions
{
    // Overlays admin-provided SEO title/description onto ViewData for the current UI culture.
    // Leaves existing ViewData["Title"] untouched when the admin hasn't filled in a value yet,
    // so pages fall back to their current resx-based title.
    public static void ApplySeoMeta(this ViewDataDictionary viewData, PageSeoMeta? seo)
    {
        if (seo == null)
            return;

        var isEnglish = CultureHelper.IsEnglish();
        var title = seo.LocalizedTitle(isEnglish);
        var description = seo.LocalizedDescription(isEnglish);

        if (!string.IsNullOrWhiteSpace(title))
            viewData["Title"] = title;

        if (!string.IsNullOrWhiteSpace(description))
            viewData["MetaDescription"] = description;
    }
}
