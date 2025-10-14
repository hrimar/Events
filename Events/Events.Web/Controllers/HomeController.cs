using System.Diagnostics;
using Events.Web.Models;
using Events.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;

        public HomeController(ILogger<HomeController> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string? category = null, bool? free = null)
        {
            try
            {
                // Get paginated events via service layer
                var (events, totalCount) = await _eventService.GetPagedEventsAsync(page, pageSize, null, category, free);

                var eventViewModels = EventViewModel.FromEntities(events);

                var paginatedEvents = new PaginatedList<EventViewModel>(eventViewModels, totalCount, page, pageSize);

                var viewModel = new HomePageViewModel
                {
                    Events = eventViewModels,
                    TotalEvents = totalCount,
                    PaginatedEvents = paginatedEvents
                };

                ViewBag.CurrentCategory = category;
                ViewBag.CurrentFree = free;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");

                var emptyViewModel = new HomePageViewModel
                {
                    PaginatedEvents = new PaginatedList<EventViewModel>(new List<EventViewModel>(), 0, 1, pageSize)
                };
                return View(emptyViewModel);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}