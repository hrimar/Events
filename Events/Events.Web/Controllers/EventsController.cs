using Events.Data.Repositories.Interfaces;
using Events.Models;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Extensions;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Models.DTOs;
using Events.Web.Resources;
using Events.Web.Services;
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
    private const int MinSearchQueryLength = 2;
    private const int MaxEventSuggestions = 8;
    private const int MaxVenueSuggestions = 3;
    private const int MaxPastSearchResults = 12;
    private const int MaxPopularTagsCount = 20;

    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;
    private readonly ITagService _tagService;
    private readonly IVenueService _venueService;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ISeoMetaService _seoMetaService;
    private readonly ISiteUrlProvider _siteUrlProvider;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ISofiaClock _clock;
    private readonly IEventsPeriodViewBuilder _periodViewBuilder;

    public EventsController(
        ILogger<EventsController> logger,
        IEventService eventService,
        ITagService tagService,
        IVenueService venueService,
        ISubCategoryRepository subCategoryRepository,
        ISeoMetaService seoMetaService,
        ISiteUrlProvider siteUrlProvider,
        IStringLocalizer<SharedResources> localizer,
        ISofiaClock clock,
        IEventsPeriodViewBuilder periodViewBuilder)
    {
        _logger = logger;
        _eventService = eventService;
        _tagService = tagService;
        _venueService = venueService;
        _subCategoryRepository = subCategoryRepository;
        _seoMetaService = seoMetaService;
        _siteUrlProvider = siteUrlProvider;
        _localizer = localizer;
        _clock = clock;
        _periodViewBuilder = periodViewBuilder;
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
        string? period = null,
        bool? kids = null,
        string? sortBy = null,
        string? sortOrder = "asc",
        CancellationToken cancellationToken = default)
    {
        try
        {
            // The start date as given in the address, before the default below (GS-REQ-008, 8.2).
            var requestedFromDate = fromDate;

            // Default to showing only future events unless explicitly specified.
            // The implicit default is not shown as an active filter.
            var hasCustomFromDate = fromDate.HasValue && fromDate.Value.Date != _clock.Today;
            fromDate ??= _clock.Today;

            // Bound caller-supplied paging to sane values - a public, unauthenticated endpoint
            // must not be able to force an oversized fetch via the pageSize query parameter.
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = DefaultPageSize;

            var tagList = SplitListParameter(tags);

            // "All events" offers only "Free only" (GS-REQ-008, 8.2.5); "Paid only" from older links is not applied.
            free = free == true ? true : null;

            // "category" accepts a comma-separated list (any of them matches); a single value works as before.
            var categoryList = SplitListParameter(category);
            var singleCategory = TryGetSingleCategory(categoryList);

            // A named period is resolved to dates on the server; explicit fromDate and toDate together win over it.
            var selectedPeriod = ResolvePeriod(period, fromDate, toDate, _clock.Today);
            var (queryFromDate, queryToDate) = selectedPeriod.HasValue
                ? EventPeriodCalculator.GetRange(selectedPeriod.Value, _clock.Today)
                : (fromDate.Value, toDate);

            // With a period, the date fields show its dates and the period itself is the active filter, not "From".
            if (selectedPeriod.HasValue)
                hasCustomFromDate = false;

            // A period or explicit dates show the events grouped by day (GS-REQ-008, 8.2); a search keeps the paged
            // list (with its "Past events" block, GS-BUG-003), the period still filters it.
            if (IsPeriodView(selectedPeriod, requestedFromDate, toDate, search))
            {
                return await PeriodViewAsync(categoryList, subCategory, free, tagList, kids == true, queryFromDate,
                    queryToDate!.Value, selectedPeriod, cancellationToken);
            }

            // Filtering, search, sorting and pagination all happen in SQL via a single query -
            // fetching the entire matching dataset and paginating in memory was the root cause
            // of the 2026-08-25 SQL worker-limit incident.
            var listQuery = new PublicEventQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = EventStatus.Published,
                CategoryNames = categoryList,
                SubCategoryName = subCategory,
                IsFree = free,
                FromDate = queryFromDate,
                ToDate = queryToDate,
                TagNames = tagList,

                // "With kids" uses the tags of the "For kids" quick filter, in addition to any selected tags (8.2.5).
                RequiredTagNames = kids == true ? HomeWeekendSelector.KidsTags : Array.Empty<string>(),
                Search = search,
                SortBy = sortBy,
                SortOrder = sortOrder ?? "asc"
            };

            var (pagedEvents, totalCount) = await _eventService.GetPagedEventsAsync(listQuery, cancellationToken);

            // An empty "next 7 days" list widens to 30 days with a notice (GS-REQ-007, 7.3.4).
            // Recomputed on every request, so paging through the widened list stays consistent.
            string? periodFallbackMessage = null;
            if (totalCount == 0 && selectedPeriod == EventPeriod.Next7)
            {
                var (_, fallbackToDate) = EventPeriodCalculator.GetFallbackRange(_clock.Today);
                listQuery = listQuery with { ToDate = fallbackToDate };
                (pagedEvents, totalCount) = await _eventService.GetPagedEventsAsync(listQuery, cancellationToken);

                if (totalCount > 0)
                    periodFallbackMessage = BuildPeriodFallbackMessage(categoryList, tagList, free);
            }

            var eventViewModels = EventViewModel.FromEntities(pagedEvents, _clock.Today);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);

            // Past matches go below the last page of upcoming results (GS-BUG-003).
            var pastSearchResults = paginatedEvents.HasNextPage
                ? new List<EventViewModel>()
                : await GetPastSearchResultsAsync(listQuery, cancellationToken);
            var popularTags = await GetPopularTagsAsync(cancellationToken);

            var pageTitle = BuildPageTitle(categoryList, free, search);

            // Default description until an admin configures PageSeoMeta for this page -
            // set before the lookup (not clobbered after), mirroring the Home page fix.
            var pageMetaDescription = _localizer["PageMetaDescription_AllEvents"].Value;

            // Per-category SEO and canonical apply only to a single category; a list is treated like "All events".
            var hasValidCategory = singleCategory.HasValue;
            var parsedCategory = singleCategory.GetValueOrDefault();

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
                PastSearchResults = pastSearchResults,
                CurrentCategory = categoryList.Count > 0 ? string.Join(",", categoryList) : null,
                SelectedCategories = categoryList,
                SelectedSubCategory = subCategory,
                IsFreeFilter = free,

                // The date fields show the dates actually listed, also for a period (e.g. tomorrow - tomorrow).
                FromDate = queryFromDate,
                ToDate = queryToDate,
                Period = selectedPeriod,
                PeriodFallbackMessage = periodFallbackMessage,
                SearchTerm = search,
                SelectedTags = tagList,
                PopularTags = popularTags,
                SortBy = sortBy,
                SortOrder = sortOrder,
                PageTitle = pageTitle,
                Heading = BuildHeading(categoryList, free, search),
                HasCustomFromDate = hasCustomFromDate,
                PageMetaDescription = pageMetaDescription
            };

            viewModel.AvailableSubCategories = await BuildSubCategoryOptionsAsync(singleCategory, subCategory);
            viewModel.LocalizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);

            // The "When" and "Filters" rows, as in the period view (GS-REQ-008, 8.2.4, 8.2.5); the default start date
            // (today) is not a filter.
            viewModel.Filters = BuildFilterState(selectedPeriod, requestedFromDate, toDate, categoryList, subCategory,
                free == true, tagList, kids == true, search);
            viewModel.Filters.LocalizedCategories = viewModel.LocalizedCategories;
            viewModel.Filters.AvailableSubCategories = viewModel.AvailableSubCategories;
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

    // "All events" grouped by day for a period or explicit dates (GS-REQ-008, 8.2).
    private async Task<IActionResult> PeriodViewAsync(
        List<string> categoryList,
        string? subCategory,
        bool? free,
        List<string> tagList,
        bool kids,
        DateTime fromDate,
        DateTime toDate,
        EventPeriod? period,
        CancellationToken cancellationToken)
    {
        // The period view has only "Free only" (8.2.5); "Paid only" from the paged list's form is not applied here.
        free = free == true ? true : null;

        var filters = BuildPeriodFilters(categoryList, subCategory, free, tagList, kids);

        // Unknown category names keep the heading without a category, as in BuildHeading.
        var categoryLabel = categoryList.Any(c => TryParseCategory(c).HasValue) ? JoinLocalizedCategoryNames(categoryList) : null;

        var (model, isFallback) = await _periodViewBuilder.BuildAsync(
            new EventsPeriodRequest(filters, fromDate, toDate, period, categoryLabel), cancellationToken);

        model.PeriodFallbackMessage = isFallback ? BuildPeriodFallbackMessage(categoryList, tagList, free, kids) : null;
        model.Filters = BuildFilterState(period, model.FromDate, model.ToDate, categoryList, subCategory, free == true,
            tagList, kids, search: null);
        model.Filters.HasFixedSort = true;
        await AddFilterMenusAsync(model.Filters, categoryList, subCategory);

        // "More N events" and the days that load on scroll ask for their cards with the same filters (8.2.8, 8.2.11).
        foreach (var day in model.Days)
            day.Route = model.Filters.RouteForDay(day.Day);

        var canonicalUrl = _siteUrlProvider.BuildAbsoluteUrl(BuildPeriodCanonicalPath(period, categoryList));

        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OgUrl"] = canonicalUrl;
        ViewData["OgImageUrl"] = _siteUrlProvider.BuildAbsoluteUrl("/images/logo.jpeg");

        return View("Period", model);
    }

    // The other events of one day of the period view, loaded in place by "More N events on Friday" (GS-REQ-008,
    // 8.2.8, gs-period.js). Takes the same filters as the period view; returns the card list items.
    [HttpGet]
    [NoIndex]
    public async Task<IActionResult> PeriodDay(
        DateTime day,
        string? category = null,
        string? subCategory = null,
        bool? free = null,
        string? tags = null,
        bool? kids = null,
        CancellationToken cancellationToken = default)
    {
        // A day that is not a date ("day=abc")
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            var filters = BuildPeriodFilters(SplitListParameter(category), subCategory, free == true ? true : null,
                SplitListParameter(tags), kids == true);

            var cards = await _periodViewBuilder.BuildRemainingCardsAsync(filters, day, cancellationToken);
            return PartialView("_PeriodDayCards", cards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the other events of {Day} for the period view", day);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // One day of the period view with its first cards, for a day that loads on scroll when the period has more than
    // 200 events (GS-REQ-008, 8.2.11, gs-period.js). Takes the same filters as the period view; returns the day section.
    [HttpGet]
    [NoIndex]
    public async Task<IActionResult> PeriodDaySection(
        DateTime day,
        string? category = null,
        string? subCategory = null,
        bool? free = null,
        string? tags = null,
        bool? kids = null,
        CancellationToken cancellationToken = default)
    {
        // A day that is not a date ("day=abc")
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            var categoryList = SplitListParameter(category);
            var tagList = SplitListParameter(tags);
            bool? freeOnly = free == true ? true : null;
            var filters = BuildPeriodFilters(categoryList, subCategory, freeOnly, tagList, kids == true);

            var model = await _periodViewBuilder.BuildDayAsync(filters, day, cancellationToken);
            if (model is null)
                return NotFound();

            var filterState = BuildFilterState(null, null, null, categoryList, subCategory, freeOnly == true, tagList,
                kids == true, search: null);
            model.Route = filterState.RouteForDay(day);

            return PartialView("_PeriodDay", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the day {Day} of the period view", day);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // The filters kept by the chips, filter buttons and day requests of "All events" (GS-REQ-008, 8.2.4, 8.2.5).
    private static EventListFilters BuildFilterState(EventPeriod? period, DateTime? fromDate, DateTime? toDate,
        List<string> categoryList, string? subCategory, bool freeOnly, List<string> tagList, bool kids, string? search) => new()
    {
        Period = period,
        FromDate = fromDate,
        ToDate = toDate,
        CurrentCategory = categoryList.Count > 0 ? string.Join(",", categoryList) : null,
        SelectedCategories = categoryList,
        SelectedSubCategory = subCategory,
        IsFreeOnly = freeOnly,
        SelectedTags = tagList,
        IsKidsFilter = kids,
        SearchTerm = search
    };

    // Options of the category and subcategory menus of the "Filters" row (8.2.5).
    private async Task AddFilterMenusAsync(EventListFilters filters, List<string> categoryList, string? subCategory)
    {
        filters.LocalizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);
        filters.AvailableSubCategories = await BuildSubCategoryOptionsAsync(TryGetSingleCategory(categoryList), subCategory);
    }

    // Filters of the period view and its "More N events" requests (GS-REQ-008, 8.2.5).
    private static PublicEventQuery BuildPeriodFilters(List<string> categoryList, string? subCategory, bool? free,
        List<string> tagList, bool kids) => new()
    {
        Status = EventStatus.Published,
        CategoryNames = categoryList,
        SubCategoryName = subCategory,
        IsFree = free,
        TagNames = tagList,

        // "With kids" uses the tags of the "For kids" quick filter, in addition to any selected tags (8.2.5).
        RequiredTagNames = kids ? HomeWeekendSelector.KidsTags : Array.Empty<string>()
    };

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
    [NoIndex]
    public async Task<IActionResult> Search(string query, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < MinSearchQueryLength)
            {
                return Json(new SearchSuggestionsDto());
            }

            var isEnglish = CultureHelper.IsEnglish();

            // Only upcoming published events, nearest date and time first (GS-BUG-003).
            var upcomingQuery = new PublicEventQuery
            {
                PageSize = MaxEventSuggestions,
                Status = EventStatus.Published,
                FromDate = _clock.Today,
                Search = query
            };
            var (events, _) = await _eventService.GetPagedEventsAsync(upcomingQuery, cancellationToken);
            var venues = await _venueService.SearchByNameAsync(query, MaxVenueSuggestions);

            return Json(new SearchSuggestionsDto
            {
                Events = events
                    .Select(e => e.ToSearchSuggestionDto(isEnglish, LocalizeCategoryName(e.Category?.Name)))
                    .ToList(),
                Venues = venues
                    .Select(v => v.ToSearchSuggestionDto(isEnglish))
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in autocomplete search for query {Query}", query);
            return Json(new SearchSuggestionsDto());
        }
    }

    private async Task<List<TagViewModel>> GetPopularTagsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var popularTags = await _tagService.GetPopularTagsAsync(_clock.Today, maxCount: MaxPopularTagsCount, cancellationToken: cancellationToken);

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

    // Past events matching the same search and filters, newest first. Empty when there is no search term.
    private async Task<List<EventViewModel>> GetPastSearchResultsAsync(PublicEventQuery listQuery, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(listQuery.Search))
        {
            return new List<EventViewModel>();
        }

        var today = _clock.Today;
        var lastPastDay = today.AddDays(-1);
        var pastQuery = listQuery with
        {
            Page = 1,
            PageSize = MaxPastSearchResults,
            FromDate = null,
            ToDate = listQuery.ToDate.HasValue && listQuery.ToDate.Value.Date < lastPastDay ? listQuery.ToDate : lastPastDay,
            SortBy = null,
            SortOrder = "desc"
        };

        var (pastEvents, _) = await _eventService.GetPagedEventsAsync(pastQuery, cancellationToken);
        return EventViewModel.FromEntities(pastEvents, today);
    }

    private string? LocalizeCategoryName(string? categoryName) =>
        string.IsNullOrEmpty(categoryName) ? null : CategoryLocalizationExtensions.LocalizeCategoryName(categoryName, _localizer);

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

            var viewModel = EventDetailsViewModel.FromEntity(eventEntity, jsonLd, _clock.Today);

            // EnumValue == OtherSubCategoryEnumValue represents "Other" across all subcategory enums
            var isOtherSubCategory = eventEntity.SubCategory == null || eventEntity.SubCategory.EnumValue == OtherSubCategoryEnumValue;
            // Fetch a few candidates (not int.MaxValue) so filtering can happen in SQL - the buffer covers the current
            // event and its other performances among the top matches (RelatedEventsSelector, GS-REQ-008, 8.1).
            List<EventViewModel> relatedEvents;
            if (!isOtherSubCategory)
            {
                // SubCategory is specific - suggest 4 events from the same SubCategory
                var result = await _eventService.GetPagedEventsAsync(new PublicEventQuery
                {
                    PageSize = RelatedEventsSelector.CandidateCount,
                    Status = EventStatus.Published,
                    CategoryNames = CategoryNamesOf(eventEntity),
                    SubCategoryName = eventEntity.SubCategory!.Name,
                    FromDate = _clock.Today
                }, cancellationToken);

                relatedEvents = EventViewModel.FromEntities(RelatedEventsSelector.Select(result.Events, eventEntity), _clock.Today).ToList();
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
                    var result = await _eventService.GetPagedEventsAsync(new PublicEventQuery
                    {
                        PageSize = RelatedEventsSelector.CandidateCount,
                        Status = EventStatus.Published,
                        CategoryNames = CategoryNamesOf(eventEntity),
                        FromDate = _clock.Today,
                        TagNames = eventTagNames
                    }, cancellationToken);

                    relatedEvents = EventViewModel.FromEntities(RelatedEventsSelector.Select(result.Events, eventEntity), _clock.Today).ToList();
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

    private string BuildPageTitle(IReadOnlyList<string> categories, bool? free, string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return $"{_localizer["PageTitle_SearchResults"]}: \"{search}\"";

        if (free == true)
            return _localizer["PageTitle_FreeEvents"];

        if (categories.Count > 0)
            return $"{JoinLocalizedCategoryNames(categories)} {_localizer["PageTitle_Events"]}";

        return _localizer["PageTitle_AllEvents"];
    }

    // Visible H1 of the listing ("All events in Sofia", "Music in Sofia"); the SEO <title> is built separately.
    private string BuildHeading(IReadOnlyList<string> categories, bool? free, string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return _localizer["Events_HeadingSearch", search.Trim()];

        // Unknown category names keep the generic heading (as before the category list was supported).
        if (categories.Count > 0 && categories.Any(c => TryParseCategory(c).HasValue))
            return _localizer["Events_HeadingCategory", JoinLocalizedCategoryNames(categories)];

        if (free == true)
            return _localizer["Events_HeadingFree"];

        return _localizer["Events_HeadingAll"];
    }

    // "Business, Conferences" -> "Бизнес, Конференции"; unknown names are shown as given.
    private string JoinLocalizedCategoryNames(IEnumerable<string> categories) =>
        string.Join(", ", categories.Select(c => TryParseCategory(c)?.Localize(_localizer) ?? c));

    // Notice above a "next 7 days" list that was widened to 30 days, naming the active filter when there is one.
    private string BuildPeriodFallbackMessage(IReadOnlyList<string> categories, IReadOnlyList<string> tags, bool? free,
        bool kids = false)
    {
        var filterLabels = categories
            .Select(c => TryParseCategory(c)?.Localize(_localizer) ?? c)
            .Concat(tags.Select(t => CategoryLocalizationExtensions.LocalizeTagName(t, _localizer)))
            .ToList();

        if (free == true)
            filterLabels.Add(_localizer["Events_FilterFreeEvents"].Value);

        if (kids)
            filterLabels.Add(_localizer["Events_FilterKids"].Value);

        return filterLabels.Count > 0
            ? _localizer["Events_PeriodFallbackFiltered", string.Join(", ", filterLabels)].Value
            : _localizer["Events_PeriodFallback"].Value;
    }

    // Splits a comma-separated query value ("Business,Conferences"), trimming blanks and duplicates.
    private static List<string> SplitListParameter(string? value) =>
        value?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new List<string>();

    // Enum.TryParse alone is not enough: it accepts numbers and combines comma-separated names into one value.
    // Canonical of the period view (GS-REQ-008, 8.3): the period and the known categories, without the other filters,
    // pages or sort. Explicit dates are not indexed (robots.txt) and keep the canonical of the paged list.
    private static string BuildPeriodCanonicalPath(EventPeriod? period, IReadOnlyList<string> categories)
    {
        var knownCategories = categories
            .Select(TryParseCategory)
            .Where(category => category.HasValue)
            .Select(category => category!.Value.ToString())
            .Distinct()
            .ToList();

        var query = new List<string>();

        if (period.HasValue)
            query.Add($"period={EventPeriodCalculator.ToQueryValue(period.Value)}");

        if (knownCategories.Count == 1 || (period.HasValue && knownCategories.Count > 1))
            query.Add($"category={string.Join(",", knownCategories)}");

        return query.Count == 0 ? "/Events" : $"/Events?{string.Join("&", query)}";
    }

    private static EventCategory? TryParseCategory(string category) =>
        Enum.TryParse<EventCategory>(category, ignoreCase: true, out var parsed)
        && Enum.IsDefined(parsed)
        && !category.Contains(',')
            ? parsed
            : null;

    private static EventCategory? TryGetSingleCategory(IReadOnlyList<string> categories) =>
        categories.Count == 1 ? TryParseCategory(categories[0]) : null;

    // The named period applies unless both explicit dates are given (they take priority). The filter form sends
    // the period's own dates back together with the hidden period field, so dates equal to the period keep it;
    // only dates the visitor changed are an explicit choice.
    private static EventPeriod? ResolvePeriod(string? period, DateTime? fromDate, DateTime? toDate, DateTime today)
    {
        if (!EventPeriodCalculator.TryParse(period, out var parsedPeriod))
            return null;

        if (!fromDate.HasValue || !toDate.HasValue)
            return parsedPeriod;

        var (periodFrom, periodTo) = EventPeriodCalculator.GetRange(parsedPeriod, today);
        return fromDate.Value.Date == periodFrom && toDate.Value.Date == periodTo ? parsedPeriod : null;
    }

    // The period view is used for a named period or for both explicit dates (from <= to, at most
    // EventsPeriodViewBuilder.MaxDays days), unless there is a search term (GS-REQ-008, 8.2).
    private static bool IsPeriodView(EventPeriod? period, DateTime? fromDate, DateTime? toDate, string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return false;

        if (period.HasValue)
            return true;

        if (!fromDate.HasValue || !toDate.HasValue || toDate.Value.Date < fromDate.Value.Date)
            return false;

        return (toDate.Value.Date - fromDate.Value.Date).TotalDays < EventsPeriodViewBuilder.MaxDays;
    }

    private static IReadOnlyList<string> CategoryNamesOf(Event eventEntity) =>
        eventEntity.Category?.Name is { } categoryName ? new[] { categoryName } : Array.Empty<string>();

    private async Task<List<SelectListItem>> BuildSubCategoryOptionsAsync(EventCategory? category, string? selectedSubCategory)
    {
        if (!category.HasValue)
        {
            return new List<SelectListItem>();
        }

        var subCategories = await _subCategoryRepository.GetByCategoryAsync(category.Value);
        return subCategories
            .Select(subCategory => new SelectListItem
            {
                Value = subCategory.Name,
                Text = CategoryLocalizationExtensions.LocalizeSubCategoryName(subCategory.Name, _localizer),
                Selected = string.Equals(subCategory.Name, selectedSubCategory, StringComparison.OrdinalIgnoreCase)
            })
            .ToList();
    }
}