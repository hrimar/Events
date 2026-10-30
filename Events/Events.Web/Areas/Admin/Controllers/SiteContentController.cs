using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class SiteContentController : Controller
{
    private const string QuickFiltersTab = "quick-filters";

    private readonly ISiteContentService _siteContentService;
    private readonly ISeoMetaService _seoMetaService;
    private readonly IHomeQuickFilterService _quickFilterService;
    private readonly ILogger<SiteContentController> _logger;

    public SiteContentController(
        ISiteContentService siteContentService,
        ISeoMetaService seoMetaService,
        IHomeQuickFilterService quickFilterService,
        ILogger<SiteContentController> logger)
    {
        _siteContentService = siteContentService;
        _seoMetaService = seoMetaService;
        _quickFilterService = quickFilterService;
        _logger = logger;
    }

    // editQuickFilterId opens the quick filter form for editing that filter (Quick filters tab).
    public async Task<IActionResult> Index(int? editQuickFilterId = null)
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
                SeoItems = seoPages
                    .OrderBy(p => GetDisplayOrder(p.PageKey))
                    .Select(p => new PageSeoMetaFormItem
                    {
                        PageKey = p.PageKey,
                        DisplayName = GetDisplayName(p.PageKey),
                        TitleBg = p.TitleBg,
                        TitleEn = p.TitleEn,
                        DescriptionBg = p.DescriptionBg,
                        DescriptionEn = p.DescriptionEn
                    }).ToList()
            };

            await LoadQuickFiltersAsync(viewModel, editQuickFilterId, submittedForm: null);

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

    // Creates a quick filter (no Id) or updates an existing one.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveQuickFilter(HomeQuickFilterFormViewModel quickFilter)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
            return await Reload(quickFilter: quickFilter);
        }

        try
        {
            var input = new HomeQuickFilterInput(
                quickFilter.LabelBg, quickFilter.LabelEn, quickFilter.Type, quickFilter.GetValues(), quickFilter.IsActive);

            var result = quickFilter.Id.HasValue
                ? await _quickFilterService.UpdateAsync(quickFilter.Id.Value, input)
                : await _quickFilterService.CreateAsync(input);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Error;
                return await Reload(quickFilter: quickFilter);
            }

            TempData["SuccessMessage"] = quickFilter.Id.HasValue ? "Quick filter updated successfully." : "Quick filter added successfully.";
            return RedirectToAction(nameof(Index), new { tab = QuickFiltersTab });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving homepage quick filter {QuickFilterId}", quickFilter.Id);
            TempData["ErrorMessage"] = "Unable to save the quick filter.";
            return await Reload(quickFilter: quickFilter);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeleteQuickFilter(int id) =>
        ChangeQuickFilterAsync(() => _quickFilterService.DeleteAsync(id), "Quick filter deleted.", id);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> SetQuickFilterActive(int id, bool isActive) =>
        ChangeQuickFilterAsync(
            () => _quickFilterService.SetActiveAsync(id, isActive),
            isActive ? "Quick filter activated." : "Quick filter deactivated.",
            id);

    // offset: -1 moves the filter up, +1 down.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> MoveQuickFilter(int id, int offset) =>
        ChangeQuickFilterAsync(() => _quickFilterService.MoveAsync(id, Math.Sign(offset)), null, id);

    // Runs a list action (delete, activate, move) and returns to the Quick filters tab with its outcome.
    private async Task<IActionResult> ChangeQuickFilterAsync(Func<Task<HomeQuickFilterResult>> change, string? successMessage, int id)
    {
        try
        {
            var result = await change();
            if (!result.Succeeded)
                TempData["ErrorMessage"] = result.Error;
            else if (successMessage != null)
                TempData["SuccessMessage"] = successMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing homepage quick filter {QuickFilterId}", id);
            TempData["ErrorMessage"] = "Unable to change the quick filter.";
        }

        return RedirectToAction(nameof(Index), new { tab = QuickFiltersTab });
    }

    // Fills the quick filter list and form; submittedForm (after a failed POST) wins over editQuickFilterId.
    private async Task LoadQuickFiltersAsync(SiteContentIndexViewModel viewModel, int? editQuickFilterId, HomeQuickFilterFormViewModel? submittedForm)
    {
        var filters = await _quickFilterService.GetAllAsync();
        viewModel.QuickFilters = filters.Select(HomeQuickFilterListItem.FromEntity).ToList();

        if (submittedForm != null)
        {
            viewModel.QuickFilterForm = submittedForm;
            viewModel.ActiveTab = QuickFiltersTab;
            return;
        }

        var filterToEdit = editQuickFilterId.HasValue ? filters.FirstOrDefault(f => f.Id == editQuickFilterId.Value) : null;
        if (filterToEdit != null)
            viewModel.QuickFilterForm = HomeQuickFilterFormViewModel.FromEntity(filterToEdit);
    }

    // Rebuilds the full Index view model when a POST fails validation/saving, preserving the
    // just-submitted values for the tab being edited and reloading the rest from the database.
    private async Task<IActionResult> Reload(
        HeroFormViewModel? hero = null,
        AboutUsFormViewModel? aboutUs = null,
        HomeQuickFilterFormViewModel? quickFilter = null)
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
            SeoItems = seoPages
                .OrderBy(p => GetDisplayOrder(p.PageKey))
                .Select(p => new PageSeoMetaFormItem
                {
                    PageKey = p.PageKey,
                    DisplayName = GetDisplayName(p.PageKey),
                    TitleBg = p.TitleBg,
                    TitleEn = p.TitleEn,
                    DescriptionBg = p.DescriptionBg,
                    DescriptionEn = p.DescriptionEn
                }).ToList()
        };

        await LoadQuickFiltersAsync(viewModel, editQuickFilterId: null, submittedForm: quickFilter);

        return View(nameof(Index), viewModel);
    }

    private static string GetDisplayName(string pageKey)
    {
        if (pageKey == Events.Models.SeoPageKeys.Home)
            return "Home";

        if (pageKey == Events.Models.SeoPageKeys.AboutUs)
            return "About Us";

        if (pageKey == Events.Models.SeoPageKeys.Contact)
            return "Contact";

        if (pageKey == Events.Models.SeoPageKeys.AllEvents)
            return "All Events (listing)";

        if (pageKey == Events.Models.SeoPageKeys.VenuesIndex)
            return "Venues (listing)";

        const string categoryPrefix = "category-";
        if (pageKey.StartsWith(categoryPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var categoryName = pageKey[categoryPrefix.Length..];
            if (Enum.TryParse<EventCategory>(categoryName, ignoreCase: true, out var category))
                return $"Category: {category}";
        }

        return pageKey;
    }

    // Display order in the admin SEO table, independent of the underlying PageSeoMeta.Id
    // (which stays append-only/immutable to avoid renumbering already-seeded rows in a
    // live database). Home/AboutUs/Contact are pinned first, in that exact order; every
    // other key keeps its existing relative (Id-ascending) order via a stable sort.
    private static int GetDisplayOrder(string pageKey)
    {
        if (pageKey == Events.Models.SeoPageKeys.Home)
            return 0;

        if (pageKey == Events.Models.SeoPageKeys.AboutUs)
            return 1;

        if (pageKey == Events.Models.SeoPageKeys.Contact)
            return 2;

        return int.MaxValue;
    }
}
