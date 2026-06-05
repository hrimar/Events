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

                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);
                var weeklyEvents = await BuildWeeklyEventsViewModelAsync(today, weekEnd, localizedCategories);

                // Stats
                var totalEvents = await _eventService.GetEventsCountInRangeAsync(today, DateTime.MaxValue, EventStatus.Published);
                var todayEvents = await _eventService.GetEventsCountInRangeAsync(today, today, EventStatus.Published);
                var next7DaysEvents = weeklyEvents.ByDay.Sum(d => d.Events.Count);

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

                    recommendedEvents = weeklyEvents.ByDay
                        .SelectMany(d => d.Events)
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

        [HttpGet]
        public async Task<IActionResult> WeeklyEventsPartial(DateTime? fromDate = null)
        {
            try
            {
                var from = (fromDate ?? DateTime.Today).Date;
                var to = from.AddDays(6);
                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);
                var weeklyEvents = await BuildWeeklyEventsViewModelAsync(from, to, localizedCategories);
                return PartialView("_WeeklyEvents", weeklyEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading weekly events partial for date {FromDate}", fromDate);
                return PartialView("_WeeklyEvents", new WeeklyEventsViewModel());
            }
        }

        private async Task<WeeklyEventsViewModel> BuildWeeklyEventsViewModelAsync(
            DateTime from,
            DateTime to,
            List<CategoryDisplayItem> localizedCategories)
        {
            // Compute weekend bounds first so we can include them in the single DB query.
            // Rule: if from is Mon–Thu ? next Fri–Sun; if from is Fri ? this Fri–Sun;
            //       if from is Sat ? last Fri–Sun (Fri+Sat+Sun); if from is Sun ? last Fri–Sun.
            var dayOfWeek = (int)from.DayOfWeek; // 0=Sun,1=Mon,...,5=Fri,6=Sat
            var weekendStart = dayOfWeek switch
            {
                0 => from.AddDays(-2),  // Sun ? Fri was 2 days ago
                6 => from.AddDays(-1),  // Sat ? Fri was yesterday
                5 => from,              // Fri ? today
                _ => from.AddDays(5 - dayOfWeek) // Mon–Thu ? next Fri
            };
            var weekendEnd = weekendStart.AddDays(2); // always Fri + 2 = Sun

            // Extend the fetch range to cover full calendar months AND the weekend.
            var fetchStart = new[] { new DateTime(from.Year, from.Month, 1), weekendStart }.Min();
            var calMonthEnd = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month));
            var fetchEnd = new[] { calMonthEnd, weekendEnd }.Max();

            var allFetchedEvents = await _eventService.GetEventsByDateRangeAsync(fetchStart, fetchEnd);
            var allPublished = EventViewModel.FromEntities(allFetchedEvents.Where(e => e.Status == EventStatus.Published).OrderBy(e => e.Date)).ToList();

            // Subset for tabs 1–2: only the selected 7-day window
            var windowEvents = allPublished.Where(e => e.Date.Date >= from && e.Date.Date <= to).ToList();

            // Tab 1: By day
            var dayCount = (int)(to - from).TotalDays + 1;
            var byDay = Enumerable.Range(0, dayCount).Select(offset =>
            {
                var date = from.AddDays(offset);
                return new DayEventsViewModel
                {
                    Date = date,
                    Events = windowEvents.Where(e => e.Date.Date == date).ToList()
                };
            }).ToList();

            // Tab 2: By category — only categories with events in the window
            var byCategory = localizedCategories
                .Select(cat => new CategoryEventsViewModel
                {
                    CategoryKey = cat.Value,
                    DisplayName = cat.DisplayName,
                    IconClass = cat.IconClass,
                    Events = windowEvents
                        .Where(e => string.Equals(e.CategoryName, cat.Value, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                })
                .Where(c => c.Events.Any())
                .ToList();

            // Tab 3: Weekend events — always the correct Fri–Sun regardless of selected window
            var weekendEvents = allPublished
                .Where(e => e.Date.Date >= weekendStart && e.Date.Date <= weekendEnd)
                .ToList();

            // Tab 4: Calendar counts for all days in the displayed calendar month(s) only
            var calendarCounts = allPublished
                .Where(e => e.Date.Date >= new DateTime(from.Year, from.Month, 1) && e.Date.Date <= calMonthEnd)
                .GroupBy(e => e.Date.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return new WeeklyEventsViewModel
            {
                ByDay = byDay,
                ByCategory = byCategory,
                WeekendEvents = weekendEvents,
                CalendarCounts = calendarCounts,
                CalendarMonth = from,
                FromDate = from,
                ToDate = to
            };
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