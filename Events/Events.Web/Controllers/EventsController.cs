using Events.Web.Models;
using Events.Services.Interfaces;
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
        DateTime? toDate = null)
    {
        try
        {
            // Set default from date to today if not specified
            fromDate ??= DateTime.Today;

            var (events, totalCount) = await _eventService.GetPagedEventsAsync(
                page, 
                pageSize, 
                Events.Models.Enums.EventStatus.Published, 
                category, 
                free, 
                fromDate);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchResults = await _eventService.SearchEventsAsync(search);
                var searchViewModels = EventViewModel.FromEntities(searchResults);
                
                var paginatedSearch = new PaginatedList<EventViewModel>(
                    searchViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                    searchViewModels.Count,
                    page,
                    pageSize);

                var searchViewModel = new EventsPageViewModel
                {
                    Events = paginatedSearch,
                    SearchTerm = search,
                    CurrentCategory = category,
                    IsFreeFilter = free,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                return View(searchViewModel);
            }

            var eventViewModels = EventViewModel.FromEntities(events);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);

            var viewModel = new EventsPageViewModel
            {
                Events = paginatedEvents,
                CurrentCategory = category,
                IsFreeFilter = free,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = search
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
            var (events, totalCount) = await _eventService.GetPagedEventsAsync(
                page, 
                pageSize, 
                Events.Models.Enums.EventStatus.Published, 
                category);

            var eventViewModels = EventViewModel.FromEntities(events);
            var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);

            var viewModel = new EventsPageViewModel
            {
                Events = paginatedEvents,
                CurrentCategory = category,
                PageTitle = $"{category} Events"
            };

            return View("Index", viewModel);
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
            var suggestions = results.Take(10).Select(e => new {
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