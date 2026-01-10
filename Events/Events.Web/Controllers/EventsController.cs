using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Events.Models.Enums;
using Events.Web.Extensions;
using Events.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Controllers;

public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;
    private readonly ITagService _tagService;
    private readonly ISubCategoryRepository _subCategoryRepository;

    public EventsController(ILogger<EventsController> logger, IEventService eventService, ITagService tagService, ISubCategoryRepository subCategoryRepository)
    {
        _logger = logger;
        _eventService = eventService;
        _tagService = tagService;
        _subCategoryRepository = subCategoryRepository;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 12,
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
                var (events, count) = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, category, free, fromDate);
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

            // Apply additional date filtering if specified
            if (toDate.HasValue)
            {
                allEvents = allEvents.Where(e => e.Date <= toDate.Value);
            }

            // Apply subcategory filtering if specified
            if (!string.IsNullOrWhiteSpace(subCategory))
            {
                allEvents = allEvents.Where(e => string.Equals(e.SubCategory?.Name, subCategory, StringComparison.OrdinalIgnoreCase));
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
                SortOrder = sortOrder
            };

            viewModel.AvailableSubCategories = await BuildSubCategoryOptionsAsync(category, subCategory);

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
    public IActionResult ByTag(string tagName, int page = 1, int pageSize = 12)
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
    public IActionResult ByTags(string tags, int page = 1, int pageSize = 12)
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
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var results = await _eventService.SearchEventsAsync(query);
            var eventSuggestions = results.Take(8)
                .Select(e => e.ToSearchSuggestionDto())
                .ToList();

            var tagResults = await SearchTagsAsync(query);
            var tagSuggestions = tagResults.Take(5)
                .Select(t => t.ToSearchSuggestionDto())
                .ToList();

            var combinedResults = eventSuggestions.Cast<object>()
                .Concat(tagSuggestions.Cast<object>())
                .Take(10)
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
                1, int.MaxValue, EventStatus.Published, null, null, DateTime.Today);
            
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
                .Take(20)
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
            var futureEvents = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, null, null, DateTime.Today);
            
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

    private static IEnumerable<Events.Models.Entities.Event> ApplySorting(
        IEnumerable<Events.Models.Entities.Event> events, 
        string? sortBy, 
        string? sortOrder)
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
                ? events.OrderByDescending(e => e.Category?.Name ?? "ZZZ")
                : events.OrderBy(e => e.Category?.Name ?? "ZZZ"),
            "subcategory" => isDescending
                ? events.OrderByDescending(e => e.SubCategory?.Name ?? "ZZZ")
                : events.OrderBy(e => e.SubCategory?.Name ?? "ZZZ"),
            "date" or _ => isDescending
                ? events.OrderByDescending(e => e.Date)
                : events.OrderBy(e => e.Date)
        };

        return sortedEvents;
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
            var relatedEvents = new List<EventViewModel>();
            
            // Try to get events with same tags first
            if (eventEntity.EventTags?.Any() == true)
            {
                var eventTags = eventEntity.EventTags.Select(et => et.Tag?.Name).Where(name => name != null).ToList();
                var tagBasedResults = await _eventService.GetAllEventsAsync();
                
                var relatedByTags = tagBasedResults
                    .Where(e => e.Id != id && 
                               e.Date >= DateTime.Today && 
                               e.Status == EventStatus.Published &&
                               e.EventTags?.Any(et => et.Tag != null && eventTags.Contains(et.Tag.Name)) == true)
                    .OrderByDescending(e => e.EventTags?.Count(et => et.Tag != null && eventTags.Contains(et.Tag.Name)) ?? 0)
                    .Take(6)
                    .ToList();
                
                relatedEvents.AddRange(EventViewModel.FromEntities(relatedByTags));
            }

            // Fill with category-based events if needed
            if (relatedEvents.Count < 3 && eventEntity.Category != null)
            {
                var related = await _eventService.GetPagedEventsAsync(1, 6, EventStatus.Published, eventEntity.Category.Name, null, DateTime.Today);
                var categoryRelated = EventViewModel.FromEntities(related.Events.Where(e => e.Id != id && !relatedEvents.Any(r => r.Id == e.Id)));
                relatedEvents.AddRange(categoryRelated);
            }

            ViewBag.RelatedEvents = relatedEvents.Take(6).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event details for ID {EventId}", id);
            return NotFound();
        }
    }

    // GET: /Events/Category/Music
    public IActionResult Category(string category, int page = 1, int pageSize = 12)
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
                Text = subCategory.Name,
                Selected = string.Equals(subCategory.Name, selectedSubCategory, StringComparison.OrdinalIgnoreCase)
            })
            .ToList();
    }
}