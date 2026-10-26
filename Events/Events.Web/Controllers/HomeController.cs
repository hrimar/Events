using System.Diagnostics;
using System.Security.Claims;
using Events.Models;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Infrastructure;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Resources;
using Events.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;

namespace Events.Web.Controllers
{
    public class HomeController : Controller
    {
        // Homepage cache section of the mobile "This week" block (see IHomePageCache).
        private const string WeeklyEventsCacheSection = "WeeklyEvents";

        // Upper bound for "Show more" in "This weekend": a public endpoint must not page without limit.
        private const int MaxWeekendSkip = 1000;

        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly ITagService _tagService;
        private readonly IUserFavoriteEventService _favoriteEventService;
        private readonly ISiteContentService _siteContentService;
        private readonly ISeoMetaService _seoMetaService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ISofiaClock _clock;
        private readonly IDeviceClassResolver _deviceClassResolver;
        private readonly IHomePageCache _homePageCache;
        private readonly IHomePageSectionsBuilder _sectionsBuilder;

        public HomeController(
            ILogger<HomeController> logger,
            IEventService eventService,
            ITagService tagService,
            IUserFavoriteEventService favoriteEventService,
            ISiteContentService siteContentService,
            ISeoMetaService seoMetaService,
            IStringLocalizer<SharedResources> localizer,
            ISofiaClock clock,
            IDeviceClassResolver deviceClassResolver,
            IHomePageCache homePageCache,
            IHomePageSectionsBuilder sectionsBuilder)
        {
            _logger = logger;
            _eventService = eventService;
            _tagService = tagService;
            _favoriteEventService = favoriteEventService;
            _siteContentService = siteContentService;
            _seoMetaService = seoMetaService;
            _localizer = localizer;
            _clock = clock;
            _deviceClassResolver = deviceClassResolver;
            _homePageCache = homePageCache;
            _sectionsBuilder = sectionsBuilder;
        }

        public async Task<IActionResult> Index()
        {
            // Phones get the mobile layout, everything else the desktop one; desktop-only sections
            // are not rendered for phones at all. The HTML differs per device class, hence Vary.
            var deviceClass = _deviceClassResolver.Resolve(Request);
            Response.Headers.Append(HeaderNames.Vary, DeviceClassResolver.VaryHeaderValue);

            try
            {
                var today = _clock.Today;
                var dayWindowEnd = today.AddDays(5);

                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);

                // Stats
                var eventCounts = await _sectionsBuilder.GetEventCountsAsync();

                // Saved + Recommended (authenticated users only)
                var savedEvents = new List<EventViewModel>();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var favorites = await _favoriteEventService.GetUserFavoritesAsync(userId);
                    savedEvents = favorites
                        .Where(f => f.Event != null && f.Event.Date >= today)
                        .Select(f => EventViewModel.FromEntity(f.Event!, today))
                        .OrderByDescending(e => e.Date)
                        .ToList();
                }

                // "This week" belongs to the mobile layout; the desktop layout replaced it (GS-REQ-007) and only
                // needs its events for the recommendations of users with saved events.
                // Contains localized category names (By category tab), so it is cached per UI culture.
                var weeklyEvents = deviceClass == DeviceClass.Mobile || savedEvents.Any()
                    ? await _homePageCache.GetOrCreateAsync(
                        HomePageCache.ForCulture(WeeklyEventsCacheSection),
                        HomePageCache.EventsDuration,
                        () => BuildWeeklyEventsViewModelAsync(today, dayWindowEnd, localizedCategories))
                    : WeeklyEventsViewModel.Empty(today);

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

                var siteContent = await _siteContentService.GetAsync();

                // Default title until an admin configures PageSeoMeta for "home" - must be
                // set here, not in the view, so ApplySeoMeta's admin override isn't clobbered
                // by an unconditional ViewData["Title"] assignment running after this action.
                ViewData["Title"] = _localizer["Home_DefaultTitle"].Value;
                var seo = await _seoMetaService.GetByKeyAsync(SeoPageKeys.Home);
                ViewData.ApplySeoMeta(seo);

                var viewModel = new HomePageViewModel
                {
                    DeviceClass = deviceClass,

                    // Desktop-only sections are not built for phones at all.
                    Hero = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildHeroAsync(siteContent) : null,
                    Tonight = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildTonightAsync() : null,
                    Weekend = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildWeekendAsync() : null,
                    DontMiss = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildDontMissAsync() : new(),
                    Interests = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildInterestTilesAsync() : new(),
                    FreeThisWeek = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildFreeThisWeekAsync() : null,
                    Venues = deviceClass == DeviceClass.Desktop ? await _sectionsBuilder.BuildVenuesAsync() : null,
                    WeeklyEvents = weeklyEvents,
                    SavedSection = EventsSectionViewModel.CreateSavedSection(
                        events: savedEvents,
                        categories: localizedCategories,
                        title: _localizer["Home_SavedEvents"]),
                    RecommendedSection = EventsSectionViewModel.CreateRecommendedSection(
                        events: recommendedEvents,
                        categories: localizedCategories,
                        title: _localizer["Home_RecommendedEvents"]),
                    TotalEvents = eventCounts.Total,
                    TodayEvents = eventCounts.Today,
                    Next7DaysEvents = eventCounts.Next7Days,
                    PopularTags = popularTags.Take(15).ToList(),
                    LocalizedCategories = localizedCategories
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View(new HomePageViewModel { DeviceClass = deviceClass, WeeklyEvents = WeeklyEventsViewModel.Empty(_clock.Today) });
            }
        }

        // Cards of "This weekend" for a day tab, a filter chip or "Show more" (GS-REQ-007, 7.6.4 and 7.6.5),
        // loaded in place by home.js. day must be a day of this weekend; filter: all, morning, evening, kids.
        [HttpGet]
        [EnableRateLimiting("events")]
        public async Task<IActionResult> WeekendEvents(DateTime day, string? filter = null, int skip = 0)
        {
            try
            {
                if (skip < 0 || skip > MaxWeekendSkip)
                    return BadRequest();

                var page = await _sectionsBuilder.GetWeekendPageAsync(day, HomeWeekendSelector.ParseFilter(filter), skip);
                return page == null ? NotFound() : PartialView("_HomeWeekendCards", page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading weekend events for {Day}, filter {Filter}, skip {Skip}", day, filter, skip);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> WeeklyEventsPartial(DateTime? fromDate = null)
        {
            try
            {
                var from = (fromDate ?? _clock.Today).Date;
                var to = from.AddDays(5);
                var localizedCategories = EventsPageViewModel.GetAvailableCategories(_localizer);
                var weeklyEvents = await BuildWeeklyEventsViewModelAsync(from, to, localizedCategories);
                return PartialView("_WeeklyEvents", weeklyEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading weekly events partial for date {FromDate}", fromDate);
                return PartialView("_WeeklyEvents", WeeklyEventsViewModel.Empty(_clock.Today));
            }
        }

        private async Task<WeeklyEventsViewModel> BuildWeeklyEventsViewModelAsync(
            DateTime from,
            DateTime to,
            List<CategoryDisplayItem> localizedCategories)
        {
            var today = _clock.Today;

            // Compute weekend bounds first so we can include them in the single DB query.
            // Rule: if from is Mon-Thu ? next Fri-Sun; if from is Fri ? this Fri-Sun;
            //       if from is Sat ? last Fri-Sun (Fri+Sat+Sun); if from is Sun ? last Fri-Sun.
            var dayOfWeek = (int)from.DayOfWeek; // 0=Sun,1=Mon,...,5=Fri,6=Sat
            var weekendStart = dayOfWeek switch
            {
                0 => from.AddDays(-2),  // Sun ? Fri was 2 days ago
                6 => from.AddDays(-1),  // Sat ? Fri was yesterday
                5 => from,              // Fri ? today
                _ => from.AddDays(5 - dayOfWeek) // Mon-Thu ? next Fri
            };
            var weekendEnd = weekendStart.AddDays(2); // always Fri + 2 = Sun

            // Extend the fetch range to cover full calendar months AND the weekend.
            var fetchStart = new[] { new DateTime(from.Year, from.Month, 1), weekendStart }.Min();
            var calMonthEnd = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month));
            var fetchEnd = new[] { calMonthEnd, weekendEnd }.Max();

            var allFetchedEvents = await _eventService.GetEventsByDateRangeAsync(fetchStart, fetchEnd);
            var allPublished = EventViewModel.FromEntities(allFetchedEvents.Where(e => e.Status == EventStatus.Published).OrderBy(e => e.Date), today).ToList();

            // Subset for tabs 1-2: only the selected 7-day window
            var windowEvents = allPublished.Where(e => e.Date.Date >= from && e.Date.Date <= to).ToList();

            // Tab 1: By day
            var dayCount = (int)(to - from).TotalDays + 1;
            var byDay = Enumerable.Range(0, dayCount).Select(offset =>
            {
                var date = from.AddDays(offset);
                return new DayEventsViewModel
                {
                    Date = date,
                    IsToday = date == today,
                    Events = windowEvents.Where(e => e.Date.Date == date).ToList()
                };
            }).ToList();

            // Tab 2: By category - only categories with events in the window
            var byCategory = localizedCategories
                .Select(cat => new CategoryEventsViewModel
                {
                    CategoryKey = cat.Value,
                    DisplayName = cat.DisplayName,
                    IconName = cat.IconName,
                    Events = windowEvents
                        .Where(e => string.Equals(e.CategoryName, cat.Value, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                })
                .Where(c => c.Events.Any())
                .ToList();

            // Tab 3: Weekend events - always the correct Fri-Sun regardless of selected window
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
                Today = today,
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

        public async Task<IActionResult> AboutUs()
        {
            ViewData["Title"] = _localizer["AboutUs_Title"];

            var isEnglish = CultureHelper.IsEnglish();
            var siteContent = await _siteContentService.GetAsync();
            var seo = await _seoMetaService.GetByKeyAsync(SeoPageKeys.AboutUs);
            ViewData.ApplySeoMeta(seo);

            var viewModel = new AboutUsPageViewModel
            {
                Content = siteContent.LocalizedAboutUsContent(isEnglish)
            };

            return View(viewModel);
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