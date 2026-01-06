using System.Diagnostics;
using Events.Web.Models;
using Events.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Events.Models.Enums;

namespace Events.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly ITagService _tagService;

        public HomeController(ILogger<HomeController> logger, IEventService eventService, ITagService tagService)
        {
            _logger = logger;
            _eventService = eventService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get only featured events for homepage - no pagination needed
                var featuredEvents = await _eventService.GetFeaturedEventsAsync(6);

                // TotalEvents: all published events from today onwards
                var totalEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.MaxValue, EventStatus.Published);

                // TodayEvents: all published events for today only
                var todayEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.Today, EventStatus.Published);

                // Next 7 days: all published events from today to today+6 (inclusive)
                var next7DaysEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.Today.AddDays(6), EventStatus.Published);

                var eventViewModels = EventViewModel.FromEntities(featuredEvents);

                // Get popular tags for homepage
                var popularTags = await GetPopularTagsAsync();

                var viewModel = new HomePageViewModel
                {
                    FeaturedEvents = eventViewModels,
                    TotalEvents = totalEvents,
                    TodayEvents = todayEvents,
                    Next7DaysEvents = next7DaysEvents,
                    PopularTags = popularTags.Take(15).ToList() // Top 15 for homepage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");

                var emptyViewModel = new HomePageViewModel
                {
                    PopularTags = new List<TagViewModel>()
                };
                return View(emptyViewModel);
            }
        }

        // Get popular tags for homepage display
        private async Task<List<TagViewModel>> GetPopularTagsAsync()
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync();
                
                return tags
                    .Where(t => t.EventTags.Any()) // Only tags with events
                    .Select(t => new TagViewModel
                    {
                        Name = t.Name,
                        EventCount = t.EventTags.Count,
                        Category = t.Category
                    })
                    .OrderByDescending(t => t.EventCount)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular tags for homepage");
                return new List<TagViewModel>();
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