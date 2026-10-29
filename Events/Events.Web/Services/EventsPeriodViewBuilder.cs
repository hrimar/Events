using System.Globalization;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Events.Web.Services;

// What the period view lists: the filters (dates are set from FromDate/ToDate), the period or explicit dates,
// and the localized category names for the H1 (null when no known category is selected).
public sealed record EventsPeriodRequest(
    PublicEventQuery Filters,
    DateTime FromDate,
    DateTime ToDate,
    EventPeriod? Period,
    string? CategoryLabel);

// Builds "All events" for a period, grouped by day (GS-REQ-008, 8.2).
public interface IEventsPeriodViewBuilder
{
    // The view model without the filter state (the controller fills it); PeriodFallbackMessage is left empty,
    // IsFallback tells whether an empty "next 7 days" was widened to 30 days (GS-REQ-007, 7.3.4).
    Task<(EventsPeriodPageViewModel Model, bool IsFallback)> BuildAsync(EventsPeriodRequest request, CancellationToken cancellationToken);

    // Cards of one day after the first PeriodDaySelector.EventsPerDay, all at once, in the same order as the day
    // section ("More N events on Friday", 8.2.8). Empty when the day has no more events.
    Task<IReadOnlyList<EventCardViewModel>> BuildRemainingCardsAsync(PublicEventQuery filters, DateTime day,
        CancellationToken cancellationToken);
}

public sealed class EventsPeriodViewBuilder : IEventsPeriodViewBuilder
{
    // Longest range of explicit dates shown grouped by day; longer ranges use the paged list.
    public const int MaxDays = 31;

    // Safety limit on the rows read for one period (about 6 times a busy month in Sofia).
    private const int MaxEventsPerPeriod = 5000;

    private readonly IEventService _eventService;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ISofiaClock _clock;

    public EventsPeriodViewBuilder(IEventService eventService, IStringLocalizer<SharedResources> localizer, ISofiaClock clock)
    {
        _eventService = eventService;
        _localizer = localizer;
        _clock = clock;
    }

    public async Task<(EventsPeriodPageViewModel Model, bool IsFallback)> BuildAsync(EventsPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var today = _clock.Today;
        var (fromDate, toDate) = (request.FromDate.Date, request.ToDate.Date);

        var keys = await _eventService.GetListingKeysAsync(
            request.Filters with { FromDate = fromDate, ToDate = toDate }, MaxEventsPerPeriod, cancellationToken);

        // An empty "next 7 days" list widens to 30 days with a notice (GS-REQ-007, 7.3.4).
        var isFallback = false;
        if (keys.Count == 0 && request.Period == EventPeriod.Next7)
        {
            var (_, fallbackToDate) = EventPeriodCalculator.GetFallbackRange(today);
            var fallbackKeys = await _eventService.GetListingKeysAsync(
                request.Filters with { FromDate = fromDate, ToDate = fallbackToDate }, MaxEventsPerPeriod, cancellationToken);

            if (fallbackKeys.Count > 0)
            {
                keys = fallbackKeys;
                toDate = fallbackToDate;
                isFallback = true;
            }
        }

        var selectedDays = PeriodDaySelector.SelectDays(keys);
        var days = await BuildDaysAsync(selectedDays, today, cancellationToken);
        var culture = CultureInfo.CurrentCulture;

        var model = new EventsPeriodPageViewModel
        {
            Heading = BuildHeading(request, culture),
            DateRangeText = EventCardFormatter.FormatWeekdayAndDateRange(fromDate, toDate, culture),
            TotalCount = keys.Count,
            TotalCountText = FormatEventCount(keys.Count),
            Days = days,
            FromDate = fromDate,
            ToDate = toDate,
            Period = request.Period
        };

        return (model, isFallback);
    }

    public async Task<IReadOnlyList<EventCardViewModel>> BuildRemainingCardsAsync(PublicEventQuery filters, DateTime day,
        CancellationToken cancellationToken)
    {
        // The same keys query as the period, for this day only, so the order and the split after the first 8 match.
        var keys = await _eventService.GetListingKeysAsync(
            filters with { FromDate = day.Date, ToDate = day.Date }, MaxEventsPerPeriod, cancellationToken);

        var ids = PeriodDaySelector.SelectRemaining(keys, day);
        if (ids.Count == 0)
            return Array.Empty<EventCardViewModel>();

        var eventsById = await LoadEventsByIdAsync(ids, cancellationToken);

        return ToCards(ids, eventsById);
    }

    private async Task<List<EventsPeriodDayViewModel>> BuildDaysAsync(IReadOnlyList<PeriodDaySelection> selectedDays,
        DateTime today, CancellationToken cancellationToken)
    {
        var ids = selectedDays.SelectMany(d => d.EventIds).ToList();
        var eventsById = await LoadEventsByIdAsync(ids, cancellationToken);
        var culture = CultureInfo.CurrentCulture;

        return selectedDays
            .Select(day => new EventsPeriodDayViewModel
            {
                Day = day.Day,
                Heading = EventCardFormatter.FormatWeekdayAndDate(day.Day, culture),
                DayLabel = day.Day == today
                    ? _localizer["Period_Today"].Value
                    : day.Day == today.AddDays(1) ? _localizer["Period_Tomorrow"].Value : null,
                IsToday = day.Day == today,
                TotalCount = day.TotalCount,
                CountText = FormatEventCount(day.TotalCount),
                Cards = ToCards(day.EventIds, eventsById),
                MoreText = day.RemainingCount > 0 ? FormatMoreOnDay(day.RemainingCount, day.Day, culture) : null
            })
            .ToList();
    }

    private async Task<Dictionary<int, EventViewModel>> LoadEventsByIdAsync(IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken)
    {
        var events = await _eventService.GetEventsByIdsAsync(ids, cancellationToken);
        return EventViewModel.FromEntities(events, _clock.Today).ToDictionary(e => e.Id);
    }

    // Cards without the date, which is in the day heading. An id missing here was deleted between the two queries;
    // its card is skipped.
    private static List<EventCardViewModel> ToCards(IEnumerable<int> ids, IReadOnlyDictionary<int, EventViewModel> eventsById) =>
        ids
            .Where(eventsById.ContainsKey)
            .Select(id => EventCardViewModel.FromEvent(eventsById[id], showDate: false))
            .ToList();

    // "Още 25 събития в петък" / "Още 1 събитие във вторник" (8.2.8)
    private string FormatMoreOnDay(int count, DateTime day, CultureInfo culture)
    {
        var onDay = EventCardFormatter.FormatOnWeekday(day, culture);
        return count == 1
            ? _localizer["Events_MoreOnDayOne", onDay].Value
            : _localizer["Events_MoreOnDay", count, onDay].Value;
    }

    // H1 by period and category (GS-REQ-008, 8.2.2): "Днес в София", "Бизнес в София: следващите 7 дни",
    // "Събития на 3 октомври"; explicit ranges: "Събития: 3–10 октомври" (not named in the spec).
    private string BuildHeading(EventsPeriodRequest request, CultureInfo culture)
    {
        var dayRange = EventCardFormatter.FormatDayRange(request.FromDate, request.ToDate, culture);

        if (request.CategoryLabel is not null)
        {
            var phrase = request.Period.HasValue ? PeriodPhrase(request.Period.Value) : dayRange;
            return _localizer["Events_PeriodHeadingCategory", request.CategoryLabel, phrase].Value;
        }

        if (request.Period.HasValue)
            return PeriodHeading(request.Period.Value);

        return request.FromDate.Date == request.ToDate.Date
            ? _localizer["Events_PeriodHeadingDate", dayRange].Value
            : _localizer["Events_PeriodHeadingRange", dayRange].Value;
    }

    private string PeriodHeading(EventPeriod period) => period switch
    {
        EventPeriod.Today => _localizer["Events_PeriodHeadingToday"].Value,
        EventPeriod.Tomorrow => _localizer["Events_PeriodHeadingTomorrow"].Value,
        EventPeriod.Weekend => _localizer["Events_PeriodHeadingWeekend"].Value,
        _ => _localizer["Events_PeriodHeadingNext7"].Value
    };

    private string PeriodPhrase(EventPeriod period) => period switch
    {
        EventPeriod.Today => _localizer["Events_PeriodPhraseToday"].Value,
        EventPeriod.Tomorrow => _localizer["Events_PeriodPhraseTomorrow"].Value,
        EventPeriod.Weekend => _localizer["Events_PeriodPhraseWeekend"].Value,
        _ => _localizer["Events_PeriodPhraseNext7"].Value
    };

    // "1 събитие" / "33 събития"
    private string FormatEventCount(int count) =>
        count == 1 ? _localizer["Events_EventCountOne"].Value : _localizer["Events_EventCount", count].Value;
}
