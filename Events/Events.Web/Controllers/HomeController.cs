using System.Diagnostics;
using System.Security.Claims;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Events.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly int _maxFeaturedEventsCount = 18;

        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly ITagService _tagService;
        private readonly IUserFavoriteEventService _favoriteEventService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public HomeController(
            ILogger<HomeController> logger,
            IEventService eventService,
            ITagService tagService,
            IUserFavoriteEventService favoriteEventService,
            IStringLocalizer<SharedResources> localizer)
        {
            _logger = logger;
            _eventService = eventService;
            _tagService = tagService;
            _favoriteEventService = favoriteEventService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get only featured events for homepage - no pagination needed
                var featuredEvents = await _eventService.GetFeaturedEventsAsync(_maxFeaturedEventsCount);

                // TotalEvents: all published events from today onwards
                var totalEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.MaxValue, EventStatus.Published);

                // TodayEvents: all published events for today only
                var todayEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.Today, EventStatus.Published);

                // Next 7 days: all published events from today to today+6 (inclusive)
                var next7DaysEvents = await _eventService.GetEventsCountInRangeAsync(DateTime.Today, DateTime.Today.AddDays(6), EventStatus.Published);

                var eventViewModels = EventViewModel.FromEntities(featuredEvents);

                // Load saved events only for authenticated users
                var savedEvents = new List<EventViewModel>();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var favorites = await _favoriteEventService.GetUserFavoritesAsync(userId);
                    savedEvents = favorites
                        .Where(f => f.Event != null)
                        .Select(f => EventViewModel.FromEntity(f.Event!))
                        .OrderByDescending(e => e.Date)
                        .ToList();
                }

                // Get popular tags for homepage
                var popularTags = await GetPopularTagsAsync();

                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);

                var viewModel = new HomePageViewModel
                {
                    FeaturedSection = EventsSectionViewModel.CreateFeaturedSection(
                        events: eventViewModels,
                        categories: localizedCategories,
                        title: _localizer["Home_FeaturedEvents"],
                        viewAllText: _localizer["Home_ViewAllEvents"],
                        viewAllUrl: Url.Action("Index", "Events")!),
                    SavedSection = EventsSectionViewModel.CreateSavedSection(
                        events: savedEvents,
                        categories: localizedCategories,
                        title: _localizer["Home_SavedEvents"]),
                    TotalEvents     = totalEvents,
                    TodayEvents     = todayEvents,
                    Next7DaysEvents = next7DaysEvents,
                    PopularTags     = popularTags.Take(15).ToList(),
                    LocalizedCategories = localizedCategories
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

        public IActionResult AboutUs()
        {
            ViewData["Title"] = _localizer["AboutUs_Title"];
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = _localizer["Privacy_Title"];
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}