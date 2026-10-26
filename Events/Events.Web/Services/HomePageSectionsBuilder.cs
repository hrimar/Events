using System.Globalization;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Extensions;
using Events.Web.Localization;
using Events.Web.Models;

namespace Events.Web.Services;

// Upcoming, today and next-7-days counts of published events.
public sealed record HomeEventCounts(int Total, int Today, int Next7Days);

// Builds the data of the homepage sections (GS-REQ-007). Database results are cached through
// IHomePageCache; texts that depend on the language or the current time are built on every request.
public interface IHomePageSectionsBuilder
{
    Task<HomeEventCounts> GetEventCountsAsync();

    // Desktop hero: date, H1/subtitle from Site Content, "When" and "What" chips, featured carousel.
    Task<HomeHeroViewModel> BuildHeroAsync(SiteContent siteContent);
}

public sealed class HomePageSectionsBuilder : IHomePageSectionsBuilder
{
    private const string EventCountsSection = "EventCounts";
    private const string FeaturedEventsSection = "FeaturedEvents";
    private const string HeroFallbackEventSection = "HeroFallbackEvent";
    private const string QuickFiltersSection = "QuickFilters";

    private const int MaxCarouselSlides = 5;

    // Enough featured performances to fill the carousel and "Don't miss" after merging repeated performances.
    private const int FeaturedEventsFetchCount = 50;

    // Upcoming events checked for the fallback slide (the first one with its own image is used).
    private const int FallbackCandidatesCount = 30;

    // Placeholder in the admin hero texts for the number of events in the next 7 days (GS-REQ-007, 7.1.3).
    private const string Next7DaysPlaceholder = "{broi7}";

    private const string DefaultEventImageUrl = "/images/default_event_image.jpeg";

    private readonly IEventService _eventService;
    private readonly IHomeQuickFilterService _quickFilterService;
    private readonly IHomePageCache _homePageCache;
    private readonly ISofiaClock _clock;

    public HomePageSectionsBuilder(
        IEventService eventService,
        IHomeQuickFilterService quickFilterService,
        IHomePageCache homePageCache,
        ISofiaClock clock)
    {
        _eventService = eventService;
        _quickFilterService = quickFilterService;
        _homePageCache = homePageCache;
        _clock = clock;
    }

    public Task<HomeEventCounts> GetEventCountsAsync() =>
        _homePageCache.GetOrCreateAsync(EventCountsSection, HomePageCache.EventsDuration, LoadEventCountsAsync);

    public async Task<HomeHeroViewModel> BuildHeroAsync(SiteContent siteContent)
    {
        var culture = CultureInfo.CurrentCulture;
        var isEnglish = CultureHelper.IsEnglish();

        var counts = await GetEventCountsAsync();
        var featuredEvents = await GetFeaturedEventsAsync();

        var slideEvents = featuredEvents.Take(MaxCarouselSlides).ToList();
        var hasFeaturedEvents = slideEvents.Count > 0;
        if (!hasFeaturedEvents)
        {
            var fallbackEvent = await _homePageCache.GetOrCreateAsync(
                HeroFallbackEventSection, HomePageCache.EventsDuration, LoadFallbackEventAsync);
            if (fallbackEvent != null)
                slideEvents.Add(fallbackEvent);
        }

        var quickFilters = await _homePageCache.GetOrCreateAsync(
            QuickFiltersSection, HomePageCache.EventsDuration, LoadQuickFiltersWithEventsAsync);

        return new HomeHeroViewModel
        {
            DateText = EventCardFormatter.FormatWeekdayAndDate(_clock.Today, culture),
            Title = ReplaceNext7DaysCount(siteContent.LocalizedHeroTitle(isEnglish), counts.Next7Days),
            Subtitle = ReplaceNext7DaysCount(siteContent.LocalizedHeroSubtitle(isEnglish), counts.Next7Days),
            TodayCount = counts.Today,
            QuickFilters = quickFilters
                .Select(f => new HomeQuickFilterChipViewModel
                {
                    Label = isEnglish ? f.LabelEn : f.LabelBg,
                    RouteValues = f.ToRouteValues(),
                    IsFree = f.Type == HomeQuickFilterType.Free
                })
                .ToList(),
            Slides = slideEvents.Select(e => ToSlide(e, culture)).ToList(),
            HasFeaturedEvents = hasFeaturedEvents
        };
    }

    // Featured events from today on, nearest first, one entry per event: repeated performances
    // (same name and venue) keep only the nearest one (decision of 25.10.2026), so one featured
    // event never fills several slides. Shared with "Don't miss" (GS-REQ-007, 7.7), which starts after the carousel.
    private Task<List<EventViewModel>> GetFeaturedEventsAsync() =>
        _homePageCache.GetOrCreateAsync(FeaturedEventsSection, HomePageCache.EventsDuration, async () =>
        {
            var events = await _eventService.GetFeaturedEventsAsync(FeaturedEventsFetchCount);

            return EventViewModel.FromEntities(events, _clock.Today)
                .DistinctBy(GetPerformanceGroupKey)
                .ToList();
        });

    private async Task<HomeEventCounts> LoadEventCountsAsync()
    {
        var today = _clock.Today;
        var (next7From, next7To) = EventPeriodCalculator.GetRange(EventPeriod.Next7, today);

        var total = await _eventService.GetEventsCountInRangeAsync(today, DateTime.MaxValue, EventStatus.Published);
        var todayCount = await _eventService.GetEventsCountInRangeAsync(today, today, EventStatus.Published);
        var next7Days = await _eventService.GetEventsCountInRangeAsync(next7From, next7To, EventStatus.Published);

        return new HomeEventCounts(total, todayCount, next7Days);
    }

    // The nearest upcoming event with its own image, for the carousel when nothing is featured (GS-REQ-007, 7.4.7).
    private async Task<EventViewModel?> LoadFallbackEventAsync()
    {
        var upcomingEvents = await _eventService.GetUpcomingEventsAsync(FallbackCandidatesCount);

        return EventViewModel.FromEntities(upcomingEvents, _clock.Today)
            .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.ImageUrl) && e.ImageUrl != DefaultEventImageUrl);
    }

    // Active chips that have at least one event in the next 30 days; the others are hidden (GS-REQ-007, 7.3.5).
    private async Task<List<HomeQuickFilter>> LoadQuickFiltersWithEventsAsync()
    {
        var (fromDate, toDate) = EventPeriodCalculator.GetFallbackRange(_clock.Today);
        var filters = await _quickFilterService.GetActiveAsync();
        var filtersWithEvents = new List<HomeQuickFilter>();

        // One query at a time: the queries share the request's DbContext.
        foreach (var filter in filters)
        {
            var query = filter.ApplyTo(new PublicEventQuery
            {
                PageSize = 1,
                Status = EventStatus.Published,
                FromDate = fromDate,
                ToDate = toDate
            });

            var (_, totalCount) = await _eventService.GetPagedEventsAsync(query);
            if (totalCount > 0)
                filtersWithEvents.Add(filter);
        }

        return filtersWithEvents;
    }

    private static HomeHeroSlideViewModel ToSlide(EventViewModel eventModel, CultureInfo culture)
    {
        var metaParts = new List<string> { EventCardFormatter.FormatDayAndMonthName(eventModel.Date, culture) };
        if (eventModel.HasKnownTime)
            metaParts.Add(eventModel.FormattedTime);
        if (!string.IsNullOrWhiteSpace(eventModel.Location))
            metaParts.Add(eventModel.Location);

        return new HomeHeroSlideViewModel
        {
            Id = eventModel.Id,
            Name = eventModel.Name,
            ImageUrl = string.IsNullOrWhiteSpace(eventModel.ImageUrl) ? DefaultEventImageUrl : eventModel.ImageUrl,
            MetaText = string.Join(" · ", metaParts)
        };
    }

    // Same event, other performance: equal name (ignoring case and extra spaces) at the same venue.
    private static string GetPerformanceGroupKey(EventViewModel eventModel)
    {
        var name = string.Join(' ', eventModel.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
        var venue = eventModel.CanonicalVenueId?.ToString(CultureInfo.InvariantCulture)
            ?? eventModel.Location.Trim().ToUpperInvariant();

        return $"{name}|{venue}";
    }

    private static string ReplaceNext7DaysCount(string text, int next7DaysCount) =>
        text.Replace(Next7DaysPlaceholder, next7DaysCount.ToString(CultureInfo.CurrentCulture), StringComparison.Ordinal);
}
