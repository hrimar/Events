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
                var today = DateTime.Today;
                var weekEnd = today.AddDays(6);

                // Single DB query for all 7-day data
                var weekEvents = await _eventService.GetEventsByDateRangeAsync(today, weekEnd);
                var publishedEvents = EventViewModel.FromEntities(
                    weekEvents.Where(e => e.Status == EventStatus.Published).OrderBy(e => e.Date))
                    .ToList();

                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);

                // Tab 1: By day
                var byDay = Enumerable.Range(0, 7).Select(offset =>
                {
                    var date = today.AddDays(offset);
                    return new DayEventsViewModel
                    {
                        Date = date,
                        Events = publishedEvents.Where(e => e.Date.Date == date).ToList()
                    };
                }).ToList();

                // Tab 2: By category - only categories that have events
                var byCategory = localizedCategories
                    .Select(cat => new CategoryEventsViewModel
                    {
                        CategoryKey = cat.Value,
                        DisplayName = cat.DisplayName,
                        IconClass = cat.IconClass,
                        Events = publishedEvents
                            .Where(e => string.Equals(e.CategoryName, cat.Value, StringComparison.OrdinalIgnoreCase))
                            .ToList()
                    })
                    .Where(c => c.Events.Any())
                    .ToList();

                // Tab 3: Weekend (Fri–Sun)
                var dayOfWeek = (int)today.DayOfWeek; // 0=Sun,1=Mon,...,5=Fri,6=Sat
                var daysToFriday = dayOfWeek switch
                {
                    0 => 5,  // Sun ? next Fri
                    1 => 4,  // Mon ? next Fri
                    2 => 3,  // Tue ? next Fri
                    3 => 2,  // Wed ? next Fri
                    4 => 1,  // Thu ? next Fri
                    5 => 0,  // Fri ? this Fri
                    6 => 0,  // Sat ? already weekend (use today)
                    _ => 0
                };
                // If Sat/Sun ? current weekend starts last Fri; if Fri ? today
                var weekendStart = dayOfWeek == 0
                    ? today.AddDays(-2)   // Sun ? Fri was 2 days ago
                    : dayOfWeek == 6
                        ? today.AddDays(-1) // Sat ? Fri was yesterday
                        : today.AddDays(daysToFriday);
                var weekendEnd = weekendStart.AddDays(2); // Fri+2 = Sun

                // Fetch weekend events separately if weekend extends beyond 7-day window
                List<EventViewModel> weekendEvents;
                if (weekendEnd <= weekEnd)
                {
                    weekendEvents = publishedEvents
                        .Where(e => e.Date.Date >= weekendStart && e.Date.Date <= weekendEnd)
                        .ToList();
                }
                else
                {
                    var extraEvents = await _eventService.GetEventsByDateRangeAsync(weekEnd.AddDays(1), weekendEnd);
                    weekendEvents = publishedEvents
                        .Where(e => e.Date.Date >= weekendStart && e.Date.Date <= weekendEnd)
                        .Concat(EventViewModel.FromEntities(
                            extraEvents.Where(e => e.Status == EventStatus.Published)))
                        .OrderBy(e => e.Date)
                        .ToList();
                }

                // Tab 4: Calendar counts for current month
                var calendarCounts = publishedEvents
                    .GroupBy(e => e.Date.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var weeklyEvents = new WeeklyEventsViewModel
                {
                    ByDay = byDay,
                    ByCategory = byCategory,
                    WeekendEvents = weekendEvents,
                    CalendarCounts = calendarCounts,
                    CalendarMonth = today
                };

                // Stats
                var totalEvents = await _eventService.GetEventsCountInRangeAsync(today, DateTime.MaxValue, EventStatus.Published);
                var todayEvents = await _eventService.GetEventsCountInRangeAsync(today, today, EventStatus.Published);
                var next7DaysEvents = publishedEvents.Count;

                // Saved + Recommended (authenticated users only)
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

                var recommendedEvents = new List<EventViewModel>();
                if (savedEvents.Any())
                {
                    var savedCategoryNames = savedEvents
                        .Select(e => e.CategoryName)
                        .Where(n => !string.IsNullOrEmpty(n))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var savedSubCategoryNames = savedEvents
                        .Select(e => e.SubCategoryName)
                        .Where(n => !string.IsNullOrEmpty(n))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var savedEventIds = savedEvents.Select(e => e.Id).ToHashSet();

                    recommendedEvents = publishedEvents
                        .Where(e => !savedEventIds.Contains(e.Id))
                        .Where(e => savedCategoryNames.Contains(e.CategoryName ?? string.Empty)
                                 || savedSubCategoryNames.Contains(e.SubCategoryName ?? string.Empty))
                        .ToList();
                }

                var popularTags = await GetPopularTagsAsync();

                var viewModel = new HomePageViewModel
                {
                    WeeklyEvents = weeklyEvents,
                    SavedSection = EventsSectionViewModel.CreateSavedSection(
                        events: savedEvents,
                        categories: localizedCategories,
                        title: _localizer["Home_SavedEvents"]),
                    RecommendedSection = EventsSectionViewModel.CreateRecommendedSection(
                        events: recommendedEvents,
                        categories: localizedCategories,
                        title: _localizer["Home_RecommendedEvents"]),
                    TotalEvents = totalEvents,
                    TodayEvents = todayEvents,
                    Next7DaysEvents = next7DaysEvents,
                    PopularTags = popularTags.Take(15).ToList(),
                    LocalizedCategories = localizedCategories
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View(new HomePageViewModel());
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