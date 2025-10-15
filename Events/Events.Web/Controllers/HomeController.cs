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

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get only featured events for homepage - no pagination needed
                var featuredEvents = await _eventService.GetFeaturedEventsAsync(6);
                var totalEvents = await _eventService.GetTotalEventsCountAsync(Events.Models.Enums.EventStatus.Published);

                // Get today's events count
                var todayEvents = await _eventService.GetPagedEventsAsync(1, 100,
                    Events.Models.Enums.EventStatus.Published, null, null, DateTime.Today);

                // Get this week's events count
                var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var weekEnd = weekStart.AddDays(7);
                var weekEvents = await _eventService.GetEventsByDateRangeAsync(weekStart, weekEnd);

                var eventViewModels = EventViewModel.FromEntities(featuredEvents);

                var viewModel = new HomePageViewModel
                {
                    FeaturedEvents = eventViewModels,
                    TotalEvents = totalEvents,
                    TodayEvents = todayEvents.Events.Count(),
                    ThisWeekEvents = weekEvents.Count()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");

                var emptyViewModel = new HomePageViewModel();
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