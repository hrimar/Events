using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class SiteContentController : Controller
{
    private readonly ISiteContentService _siteContentService;
    private readonly ISeoMetaService _seoMetaService;
    private readonly ILogger<SiteContentController> _logger;

    public SiteContentController(ISiteContentService siteContentService, ISeoMetaService seoMetaService, ILogger<SiteContentController> logger)
    {
        _siteContentService = siteContentService;
        _seoMetaService = seoMetaService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var siteContent = await _siteContentService.GetAsync();
            var seoPages = await _seoMetaService.GetAllAsync();

            var viewModel = new SiteContentIndexViewModel
            {
                Hero = new HeroFormViewModel
                {
                    HeroTitleBg = siteContent.HeroTitleBg,
                    HeroTitleEn = siteContent.HeroTitleEn,
                    HeroSubtitleBg = siteContent.HeroSubtitleBg,
                    HeroSubtitleEn = siteContent.HeroSubtitleEn
                },
                AboutUs = new AboutUsFormViewModel
                {
                    AboutUsContentBg = siteContent.AboutUsContentBg,
                    AboutUsContentEn = siteContent.AboutUsContentEn
                },
                SeoItems = seoPages.Select(p => new PageSeoMetaFormItem
                {
                    PageKey = p.PageKey,
                    DisplayName = GetDisplayName(p.PageKey),
                    TitleBg = p.TitleBg,
                    TitleEn = p.TitleEn,
                    DescriptionBg = p.DescriptionBg,
                    DescriptionEn = p.DescriptionEn
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading site content");
            TempData["ErrorMessage"] = "Unable to load site content.";
            return View(new SiteContentIndexViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveHero(HeroFormViewModel hero)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
            return await Reload(hero: hero);
        }

        try
        {
            await _siteContentService.UpdateHeroAsync(hero.HeroTitleBg, hero.HeroTitleEn, hero.HeroSubtitleBg, hero.HeroSubtitleEn);
            TempData["SuccessMessage"] = "Hero content updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "hero" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving hero content");
            TempData["ErrorMessage"] = "Unable to save hero content.";
            return await Reload(hero: hero);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAboutUs(AboutUsFormViewModel aboutUs)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
            return await Reload(aboutUs: aboutUs);
        }

        try
        {
            await _siteContentService.UpdateAboutUsAsync(aboutUs.AboutUsContentBg, aboutUs.AboutUsContentEn);
            TempData["SuccessMessage"] = "About Us content updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = "about-us" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving About Us content");
            TempData["ErrorMessage"] = "Unable to save About Us content.";
            return await Reload(aboutUs: aboutUs);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSeo(List<PageSeoMetaFormItem> seoItems)
    {
        try
        {
            var pages = seoItems.Select(item => new PageSeoMeta
            {
                PageKey = item.PageKey,
                TitleBg = item.TitleBg,
                TitleEn = item.TitleEn,
                DescriptionBg = item.DescriptionBg,
                DescriptionEn = item.DescriptionEn
            });

            await _seoMetaService.SaveAllAsync(pages);
            TempData["SuccessMessage"] = "SEO meta updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving SEO meta");
            TempData["ErrorMessage"] = "Unable to save SEO meta.";
        }

        return RedirectToAction(nameof(Index), new { tab = "seo" });
    }

    // Rebuilds the full Index view model when a POST fails validation/saving, preserving the
    // just-submitted values for the tab being edited and reloading the rest from the database.
    private async Task<IActionResult> Reload(HeroFormViewModel? hero = null, AboutUsFormViewModel? aboutUs = null)
    {
        var siteContent = await _siteContentService.GetAsync();
        var seoPages = await _seoMetaService.GetAllAsync();

        var viewModel = new SiteContentIndexViewModel
        {
            Hero = hero ?? new HeroFormViewModel
            {
                HeroTitleBg = siteContent.HeroTitleBg,
                HeroTitleEn = siteContent.HeroTitleEn,
                HeroSubtitleBg = siteContent.HeroSubtitleBg,
                HeroSubtitleEn = siteContent.HeroSubtitleEn
            },
            AboutUs = aboutUs ?? new AboutUsFormViewModel
            {
                AboutUsContentBg = siteContent.AboutUsContentBg,
                AboutUsContentEn = siteContent.AboutUsContentEn
            },
            SeoItems = seoPages.Select(p => new PageSeoMetaFormItem
            {
                PageKey = p.PageKey,
                DisplayName = GetDisplayName(p.PageKey),
                TitleBg = p.TitleBg,
                TitleEn = p.TitleEn,
                DescriptionBg = p.DescriptionBg,
                DescriptionEn = p.DescriptionEn
            }).ToList()
        };

        return View(nameof(Index), viewModel);
    }

    private static string GetDisplayName(string pageKey)
    {
        if (pageKey == Events.Models.SeoPageKeys.Home)
            return "Home";

        if (pageKey == Events.Models.SeoPageKeys.AboutUs)
            return "About Us";

        const string categoryPrefix = "category-";
        if (pageKey.StartsWith(categoryPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var categoryName = pageKey[categoryPrefix.Length..];
            if (Enum.TryParse<EventCategory>(categoryName, ignoreCase: true, out var category))
                return $"Category: {category}";
        }

        return pageKey;
    }
}
