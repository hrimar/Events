using Events.Data.Repositories.Interfaces;
using Events.Models;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Extensions;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Models.DTOs;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace Events.Web.Controllers;

[EnableRateLimiting("events")]
public class EventsController : Controller
{
    private const int OtherSubCategoryEnumValue = 99;
    private const int DefaultPageSize = 12;
    private const int RelatedEventsCount = 4;
    private const int MinSearchQueryLength = 2;
    private const int MaxEventSuggestions = 8;
    private const int MaxTagSuggestions = 5;
    private const int MaxAutocompleteSuggestions = 10;
    private const int MaxPopularTagsCount = 20;

    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;
    private readonly ITagService _tagService;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ISeoMetaService _seoMetaService;
    private readonly ISiteUrlProvider _siteUrlProvider;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public EventsController(
        ILogger<EventsController> logger,
        IEventService eventService,
        ITagService tagService,
        ISubCategoryRepository subCategoryRepository,
        ISeoMetaService seoMetaService,
        ISiteUrlProvider siteUrlProvider,
        IStringLocalizer<SharedResources> localizer)
    {
        _logger = logger;
        _eventService = eventService;
        _tagService = tagService;
        _subCategoryRepository = subCategoryRepository;
        _seoMetaService = seoMetaService;
        _siteUrlProvider = siteUrlProvider;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = DefaultPageSize,
        string? category = null,
        string? subCategory = null,
        bool? free = null,
        string? search = null,
        string? tags = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? sortBy = null,
        string? sortOrder = "asc",
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Default to showing only future events unless explicitly specified
            fromDate ??= DateTime.Today;

            // Bound caller-supplied paging to sane values - a public, unauthenticated endpoint
            // must not be able to force an oversized fetch via the pageSize query parameter.
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = DefaultPageSize;

            var tagList = tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList() ?? new List<string>();

            List<Events.Models.Entities.Event> pagedEvents;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Search results come from a separate lookup and are filtered/sorted/paged in memory
                // (pre-existing behavior, unchanged here).
                IEnumerable<Events.Models.Entities.Event> allEvents = await _eventService.SearchEventsAsync(search, cancellationToken);

                if (tagList.Count > 0)
                {
                    allEvents = allEvents.Where(e =>
                        e.EventTags != null && e.EventTags.Any(et =>
                            et.Tag != null && tagList.Any(searchTag =>
                                string.Equals(et.Tag.Name.Trim(), searchTag.Trim(), StringComparison.OrdinalIgnoreCase))));
                }

                // Use exclusive upper bound (< next day) so events at any time during toDate are included.
                if (toDate.HasValue)
                {
                    var exclusiveEnd = toDate.Value.Date.AddDays(1);
                    allEvents = allEvents.Where(e => e.Date < exclusiveEnd);
                }

                allEvents = ApplySorting(allEvents, sortBy, sortOrder);

                totalCount = allEvents.Count();
                pagedEvents = allEvents
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                // Filtering, sorting and pagination all happen in SQL via a single query -
                // this used to fetch the entire matching dataset (pageSize = int.MaxValue) and
                // paginate in memory, which was the root cause of the 2026-08-25 SQL worker-limit incident.
                var (events, count) = await _eventService.GetPagedEventsAsync(
                    page, pageSize, EventStatus.Published, category, subCategory, free, fromDate, sortBy, sortOrder ?? "asc", toDate, tagList, cancellationToken);
                pagedEvents = events.ToList();
                totalCount = count;
            }

            var eventViewModels = EventViewModel.FromEntities(pagedEvents);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);
            var popularTags = await GetPopularTagsAsync(cancellationToken);

            var pageTitle = BuildPageTitle(category, free, search);

            // Default description until an admin configures PageSeoMeta for this page -
            // set before the lookup (not clobbered after), mirroring the Home page fix.
            var pageMetaDescription = _localizer["PageMetaDescription_AllEvents"].Value;

            EventCategory parsedCategory = default;
            var hasValidCategory = !string.IsNullOrWhiteSpace(category) && Enum.TryParse(category, ignoreCase: true, out parsedCategory);

            // "All events" (no category) has its own PageSeoMeta key (SeoPageKeys.AllEvents),
            // separate from the per-category keys, so an admin can configure both independently.
            var seoKey = hasValidCategory ? SeoPageKeys.ForCategory(parsedCategory) : SeoPageKeys.AllEvents;
            var seo = await _seoMetaService.GetByKeyAsync(seoKey);
            if (seo != null)
            {
                var isEnglish = CultureHelper.IsEnglish();
                var seoTitle = seo.LocalizedTitle(isEnglish);
                if (!string.IsNullOrWhiteSpace(seoTitle))
                    pageTitle = seoTitle;

                var seoDescription = seo.LocalizedDescription(isEnglish);
                if (!string.IsNullOrWhiteSpace(seoDescription))
                    pageMetaDescription = seoDescription;
            }

            // Category listing pages are individually listed in sitemap.xml (see SeoController),
            // so they must self-canonicalize to their own "?category=" URL, not the bare /Events -
            // otherwise Google is told two contradictory things about the same page.
            var canonicalUrl = hasValidCategory
                ? _siteUrlProvider.BuildAbsoluteUrl($"/Events?category={parsedCategory}")
                : _siteUrlProvider.BuildAbsoluteUrl("/Events");

            ViewData["CanonicalUrl"] = canonicalUrl;
            ViewData["OgUrl"] = canonicalUrl;
            ViewData["OgImageUrl"] = _siteUrlProvider.BuildAbsoluteUrl("/images/logo.jpeg");

            var viewModel = new EventsPageViewModel
            {
                Events = paginatedEvents,
                CurrentCategory = category,
                SelectedSubCategory = subCategory,
                IsFreeFilter = free,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = search,
                SelectedTags = tagList,
                PopularTags = popularTags,
                SortBy = sortBy,
                SortOrder = sortOrder,
                PageTitle = pageTitle,
                PageMetaDescription = pageMetaDescription
            };

            viewModel.AvailableSubCategories = await BuildSubCategoryOptionsAsync(category, subCategory);
            viewModel.LocalizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);
            viewModel.LocalizedSortOptions = EventsPageViewModel.GetAvailableSortOptions(_localizer);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading events page");

            var emptyViewModel = new EventsPageViewModel
            {
                Events = new PaginatedList<EventViewModel>(new List<EventViewModel>(), 0, 1, pageSize),
                PopularTags = new List<TagViewModel>()
            };
            return View(emptyViewModel);
        }
    }

    [HttpGet("/Events/Tag/{tagName}")]
    public IActionResult ByTag(string tagName, int page = 1, int pageSize = DefaultPageSize)
    {
        var decodedTagName = Uri.UnescapeDataString(tagName).Trim();
        _logger.LogInformation("Tag filtering requested for: '{TagName}' (decoded: '{DecodedTagName}')", tagName, decodedTagName);

        var redirectDto = new TagRedirectDto
        {
            Tags = decodedTagName,
            Page = page,
            PageSize = pageSize
        };

        return RedirectToAction(nameof(Index), redirectDto);
    }

    [HttpGet("/Events/Tags")]
    public IActionResult ByTags(string tags, int page = 1, int pageSize = DefaultPageSize)
    {
        var redirectDto = new TagRedirectDto
        {
            Tags = tags,
            Page = page,
            PageSize = pageSize
        };

        return RedirectToAction(nameof(Index), redirectDto);
    }

    [HttpGet("/Events/Search")]
    public async Task<IActionResult> Search(string query, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < MinSearchQueryLength)
            {
                return Json(Array.Empty<object>());
            }

            var results = await _eventService.SearchEventsAsync(query, cancellationToken);
            var eventSuggestions = results.Take(MaxEventSuggestions)
                .Select(e => e.ToSearchSuggestionDto())
                .ToList();

            var tagResults = await SearchTagsAsync(query, cancellationToken);
            var tagSuggestions = tagResults.Take(MaxTagSuggestions)
                .Select(t => t.ToSearchSuggestionDto())
                .ToList();

            var combinedResults = eventSuggestions.Cast<object>()
                .Concat(tagSuggestions.Cast<object>())
                .Take(MaxAutocompleteSuggestions)
                .ToList();

            return Json(combinedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in autocomplete search for query {Query}", query);
            return Json(Array.Empty<object>());
        }
    }

    private async Task<List<TagViewModel>> GetPopularTagsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var popularTags = await _tagService.GetPopularTagsAsync(DateTime.Today, maxCount: MaxPopularTagsCount, cancellationToken: cancellationToken);

            return popularTags
                .Select(t => new TagViewModel { Name = t.Name, EventCount = t.EventCount, Category = t.Category })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            return new List<TagViewModel>();
        }
    }

    private async Task<List<TagViewModel>> SearchTagsAsync(string query, CancellationToken cancellationToken)
    {
        try
        {
            var matchingTags = await _tagService.GetPopularTagsAsync(DateTime.Today, nameFilter: query.Trim(), cancellationToken: cancellationToken);

            return matchingTags
                .Select(t => new TagViewModel { Name = t.Name, EventCount = t.EventCount, Category = t.Category })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags for query {Query}", query);
            return new List<TagViewModel>();
        }
    }

    // GET: /Events/Details/5
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id, cancellationToken);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var baseUrl = _siteUrlProvider.BaseUrl;
            var jsonLd = SafeJsonLdBuilder.Serialize(SafeJsonLdBuilder.BuildGraph(
                EventJsonLdBuilder.BuildEvent(eventEntity, baseUrl, includeContext: false),
                BreadcrumbJsonLdBuilder.BuildBreadcrumbList(BuildBreadcrumbItems(eventEntity, baseUrl), includeContext: false)));

            var viewModel = EventDetailsViewModel.FromEntity(eventEntity, jsonLd);

            // EnumValue == OtherSubCategoryEnumValue represents "Other" across all subcategory enums
            var isOtherSubCategory = eventEntity.SubCategory == null || eventEntity.SubCategory.EnumValue == OtherSubCategoryEnumValue;
            // Fetch RelatedEventsCount + 1 (not int.MaxValue) so filtering can happen in SQL - the
            // +1 buffer covers the case where the current event itself is among the top matches.
            List<EventViewModel> relatedEvents;
            if (!isOtherSubCategory)
            {
                // SubCategory is specific - suggest 4 events from the same SubCategory
                var result = await _eventService.GetPagedEventsAsync(1, RelatedEventsCount + 1, EventStatus.Published,
                    eventEntity.Category?.Name, eventEntity.SubCategory!.Name, null, DateTime.Today, cancellationToken: cancellationToken);

                relatedEvents = EventViewModel.FromEntities(result.Events.Where(e => e.Id != id).Take(RelatedEventsCount)).ToList();
            }
            else
            {
                // SubCategory is null or "Other" - suggest 4 events from same Category with at least one matching tag
                var eventTagNames = eventEntity.EventTags
                    .Select(et => et.Tag?.Name)
                    .Where(name => name != null)
                    .Select(name => name!)
                    .ToList();

                if (eventTagNames.Count == 0)
                {
                    relatedEvents = new List<EventViewModel>();
                }
                else
                {
                    var result = await _eventService.GetPagedEventsAsync(1, RelatedEventsCount + 1, EventStatus.Published,
                        eventEntity.Category?.Name, null, null, DateTime.Today, tagNames: eventTagNames, cancellationToken: cancellationToken);

                    relatedEvents = EventViewModel.FromEntities(result.Events.Where(e => e.Id != id).Take(RelatedEventsCount)).ToList();
                }
            }

            ViewBag.RelatedEvents = relatedEvents;
            ViewBag.IsOtherSubCategory = isOtherSubCategory;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event details for ID {EventId}", id);
            return NotFound();
        }
    }

    // GET: /Events/Category/Music
    public IActionResult Category(string category, int page = 1, int pageSize = DefaultPageSize)
    {
        try
        {
            var redirectDto = new CategoryRedirectDto
            {
                Category = category,
                Page = page,
                PageSize = pageSize
            };

            return RedirectToAction(nameof(Index), redirectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category page for {Category}", category);
            return RedirectToAction(nameof(Index));
        }
    }

    // Mirrors the visible <nav aria-label="breadcrumb"> markup in Events/Details.cshtml
    // (Home > Events > [Category] > EventName) so JSON-LD and the on-page trail agree.
    // The category link uses the "?category=" query filter (Index's working convention,
    // same as the sitemap - see SeoController) rather than the separate Category action.
    private List<(string Name, string? Url)> BuildBreadcrumbItems(Event eventEntity, string baseUrl)
    {
        var items = new List<(string Name, string? Url)>
        {
            (_localizer["Details_Home"].Value, $"{baseUrl}/"),
            (_localizer["Details_Events"].Value, $"{baseUrl}/Events")
        };

        if (!string.IsNullOrEmpty(eventEntity.Category?.Name))
        {
            items.Add((
                CategoryLocalizationExtensions.LocalizeCategoryName(eventEntity.Category.Name, _localizer),
                $"{baseUrl}/Events?category={eventEntity.Category.Name}"));
        }

        items.Add((eventEntity.Name, null));

        return items;
    }

    private string BuildPageTitle(string? category, bool? free, string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return $"{_localizer["PageTitle_SearchResults"]}: \"{search}\"";

        if (free == true)
            return _localizer["PageTitle_FreeEvents"];

        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<EventCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            return $"{parsedCategory.Localize(_localizer)} {_localizer["PageTitle_Events"]}";
        }

        if (!string.IsNullOrWhiteSpace(category))
            return $"{category} {_localizer["PageTitle_Events"]}";

        return _localizer["PageTitle_AllEvents"];
    }

    private async Task<List<SelectListItem>> BuildSubCategoryOptionsAsync(string? category, string? selectedSubCategory)
    {
        if (string.IsNullOrWhiteSpace(category) || !Enum.TryParse<EventCategory>(category, true, out var parsedCategory))
        {
            return new List<SelectListItem>();
        }

        var subCategories = await _subCategoryRepository.GetByCategoryAsync(parsedCategory);
        return subCategories
            .Select(subCategory => new SelectListItem
            {
                Value = subCategory.Name,
                Text = CategoryLocalizationExtensions.LocalizeSubCategoryName(subCategory.Name, _localizer),
                Selected = string.Equals(subCategory.Name, selectedSubCategory, StringComparison.OrdinalIgnoreCase)
            })
            .ToList();
    }

    private static IEnumerable<Events.Models.Entities.Event> ApplySorting(IEnumerable<Events.Models.Entities.Event> events, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        var sortedEvents = sortBy?.ToLower() switch
        {
            "name" => isDescending
                ? events.OrderByDescending(e => e.Name)
                : events.OrderBy(e => e.Name),
            "price" => isDescending
                ? events.OrderByDescending(e => e.IsFree ? 0 : (e.Price ?? decimal.MaxValue))
                : events.OrderBy(e => e.IsFree ? 0 : (e.Price ?? decimal.MaxValue)),
            "category" => isDescending
                ? events.OrderByDescending(e => e.Category?.Name ?? "")
                : events.OrderBy(e => e.Category?.Name ?? ""),
            "subcategory" => isDescending
                ? events.OrderByDescending(e => e.SubCategory?.Name ?? "")
                : events.OrderBy(e => e.SubCategory?.Name ?? ""),
            "date" or _ => isDescending
                ? events.OrderByDescending(e => e.Date)
                : events.OrderBy(e => e.Date)
        };

        return sortedEvents;
    }
}