using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Extensions;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Models.DTOs;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Events.Web.Controllers;

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
    private readonly IStringLocalizer<SharedResources> _localizer;

    public EventsController(
        ILogger<EventsController> logger,
        IEventService eventService,
        ITagService tagService,
        ISubCategoryRepository subCategoryRepository,
        IStringLocalizer<SharedResources> localizer)
    {
        _logger = logger;
        _eventService = eventService;
        _tagService = tagService;
        _subCategoryRepository = subCategoryRepository;
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
        string? sortOrder = "asc")
    {
        try
        {
            // Default to showing only future events unless explicitly specified
            fromDate ??= DateTime.Today;

            IEnumerable<Events.Models.Entities.Event> allEvents;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchResults = await _eventService.SearchEventsAsync(search);
                allEvents = searchResults;
            }
            else
            {
                var (events, count) = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, category, subCategory, free, fromDate);
                allEvents = events;
            }

            // Apply tag filtering if specified
            if (!string.IsNullOrWhiteSpace(tags))
            {
                var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

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

            var totalCount = allEvents.Count();
            var pagedEvents = allEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var eventViewModels = EventViewModel.FromEntities(pagedEvents);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);
            var popularTags = await GetPopularTagsAsync();

            var pageTitle = BuildPageTitle(category, free, search);

            var viewModel = new EventsPageViewModel
            {
                Events = paginatedEvents,
                CurrentCategory = category,
                SelectedSubCategory = subCategory,
                IsFreeFilter = free,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = search,
                SelectedTags = tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList() ?? new List<string>(),
                PopularTags = popularTags,
                SortBy = sortBy,
                SortOrder = sortOrder,
                PageTitle = pageTitle
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
    public async Task<IActionResult> Search(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < MinSearchQueryLength)
            {
                return Json(Array.Empty<object>());
            }

            var results = await _eventService.SearchEventsAsync(query);
            var eventSuggestions = results.Take(MaxEventSuggestions)
                .Select(e => e.ToSearchSuggestionDto())
                .ToList();

            var tagResults = await SearchTagsAsync(query);
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

    private async Task<List<TagViewModel>> GetPopularTagsAsync()
    {
        try
        {
            var futureEvents = await _eventService.GetPagedEventsAsync(
                1, int.MaxValue, EventStatus.Published, null, null, null, DateTime.Today);

            var futureEventIds = futureEvents.Events.Select(e => e.Id).ToList();
            var allTags = await _tagService.GetAllTagsAsync();

            return allTags
                .Where(t => t.EventTags?.Any(et => futureEventIds.Contains(et.EventId)) == true)
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    EventCount = t.EventTags?.Count(et => futureEventIds.Contains(et.EventId)) ?? 0,
                    Category = t.Category
                })
                .Where(t => t.EventCount > 0)
                .OrderByDescending(t => t.EventCount)
                .Take(MaxPopularTagsCount)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            return new List<TagViewModel>();
        }
    }

    private async Task<List<TagViewModel>> SearchTagsAsync(string query)
    {
        try
        {
            var futureEvents = await _eventService.GetPagedEventsAsync(
                1, int.MaxValue, EventStatus.Published, null, null, null, DateTime.Today);

            var futureEventIds = futureEvents.Events.Select(e => e.Id).ToList();
            var allTags = await _tagService.GetAllTagsAsync();

            var matchingTags = allTags
                .Where(t => t.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(t => t.EventTags?.Any(et => futureEventIds.Contains(et.EventId)) == true)
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    EventCount = t.EventTags?.Count(et => futureEventIds.Contains(et.EventId)) ?? 0,
                    Category = t.Category
                })
                .Where(t => t.EventCount > 0)
                .OrderByDescending(t => t.EventCount)
                .ToList();

            return matchingTags;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags for query {Query}", query);
            return new List<TagViewModel>();
        }
    }

    // GET: /Events/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var viewModel = EventViewModel.FromEntity(eventEntity);

            // EnumValue == OtherSubCategoryEnumValue represents "Other" across all subcategory enums
            var isOtherSubCategory = eventEntity.SubCategory == null || eventEntity.SubCategory.EnumValue == OtherSubCategoryEnumValue;
            List<EventViewModel> relatedEvents;
            if (!isOtherSubCategory)
            {
                // SubCategory is specific - suggest 4 events from the same SubCategory
                var result = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published,
                    eventEntity.Category?.Name, eventEntity.SubCategory!.Name, null, DateTime.Today);

                relatedEvents = EventViewModel.FromEntities(result.Events.Where(e => e.Id != id).Take(RelatedEventsCount)).ToList();
            }
            else
            {
                // SubCategory is null or "Other" - suggest 4 events from same Category with at least one matching tag
                var eventTagNames = eventEntity.EventTags
                    .Select(et => et.Tag?.Name)
                    .Where(name => name != null)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)!;

                var result = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, eventEntity.Category?.Name,
                    null, null, DateTime.Today);

                relatedEvents = EventViewModel.FromEntities(result.Events
                        .Where(e => e.Id != id && e.EventTags?.Any(et => et.Tag != null && eventTagNames.Contains(et.Tag.Name)) == true)
                        .Take(RelatedEventsCount))
                    .ToList();
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