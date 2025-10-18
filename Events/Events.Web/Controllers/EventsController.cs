using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;

    public EventsController(ILogger<EventsController> logger, IEventService eventService)
    {
        _logger = logger;
        _eventService = eventService;
    }

    // GET: /Events
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 12,
        string? category = null,
        bool? free = null,
        string? search = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? sortBy = null,
        string? sortOrder = "asc")
    {
        try
        {
            fromDate ??= DateTime.Today;

            // Get ALL matching events first (not paginated)
            IEnumerable<Event> allEvents;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(search))
            {
                // If searching, get search results
                var searchResults = await _eventService.SearchEventsAsync(search);
                allEvents = searchResults;
            }
            else
            {
                // Otherwise get all published events
                var (events, count) = await _eventService.GetPagedEventsAsync(
                    1, // Get first page to start with
                    int.MaxValue, // Get all events for sorting
                    Events.Models.Enums.EventStatus.Published,
                    category,
                    free,
                    fromDate);
                allEvents = events;
            }

            // Apply additional filters
            if (toDate.HasValue)
            {
                allEvents = allEvents.Where(e => e.Date <= toDate.Value);
            }

            // Apply sorting BEFORE pagination
            allEvents = ApplySorting(allEvents, sortBy, sortOrder);

            // Get total count after all filtering
            totalCount = allEvents.Count();

            // Apply pagination AFTER sorting
            var pagedEvents = allEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var eventViewModels = EventViewModel.FromEntities(pagedEvents);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);

            var viewModel = new EventsPageViewModel
            {
                Events = paginatedEvents,
                CurrentCategory = category,
                IsFreeFilter = free,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = search,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading events page");

            var emptyViewModel = new EventsPageViewModel
            {
                Events = new PaginatedList<EventViewModel>(new List<EventViewModel>(), 0, 1, pageSize)
            };
            return View(emptyViewModel);
        }
    }

    // Improved sorting method with better defaults
    private IEnumerable<Event> ApplySorting(IEnumerable<Event> events, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";
        
        _logger.LogInformation("Applying sort: {SortBy} {SortOrder} (isDescending: {IsDescending})", 
            sortBy, sortOrder, isDescending);

        var sortedEvents = sortBy?.ToLower() switch
        {
            "name" => isDescending
                ? events.OrderByDescending(e => e.Name)
                : events.OrderBy(e => e.Name),
            "price" => isDescending
                ? events.OrderByDescending(e => e.IsFree ? 0 : (e.Price ?? decimal.MaxValue))
                : events.OrderBy(e => e.IsFree ? 0 : (e.Price ?? decimal.MaxValue)),
            "category" => isDescending
                ? events.OrderByDescending(e => e.Category?.Name ?? "ZZZ") // Put null categories at end
                : events.OrderBy(e => e.Category?.Name ?? "ZZZ"),
            "date" or _ => isDescending
                ? events.OrderByDescending(e => e.Date)
                : events.OrderBy(e => e.Date) // Default sort by date ascending
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

            // Get related events in same category
            var relatedEvents = new List<EventViewModel>();
            if (eventEntity.Category != null)
            {
                var related = await _eventService.GetPagedEventsAsync(1, 6, null, eventEntity.Category.Name);
                relatedEvents = EventViewModel.FromEntities(related.Events.Where(e => e.Id != id));
            }

            ViewBag.RelatedEvents = relatedEvents;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event details for ID {EventId}", id);
            return NotFound();
        }
    }

    // GET: /Events/Category/Music
    public async Task<IActionResult> Category(string category, int page = 1, int pageSize = 12)
    {
        try
        {
            // Redirect to Index with category filter
            return RedirectToAction(nameof(Index), new { category = category, page = page, pageSize = pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category page for {Category}", category);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet("/Events/Search")]
    public async Task<IActionResult> Search(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new object[0]);
            }

            var results = await _eventService.SearchEventsAsync(query);
            var suggestions = results.Take(10).Select(e => new
            {
                id = e.Id,
                name = e.Name,
                category = e.Category?.Name,
                location = e.Location,
                date = e.Date.ToString("dd.MM.yyyy")
            });

            return Json(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in autocomplete search for query {Query}", query);
            return Json(new object[0]);
        }
    }
}