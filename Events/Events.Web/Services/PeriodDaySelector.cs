using Events.Models.DTOs;
using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Services;

// One day of the period view: its date, how many events it has and the ids of the events to show, in order.
public sealed record PeriodDaySelection(DateTime Day, int TotalCount, IReadOnlyList<int> EventIds)
{
    // Events of the day that are not shown yet ("More N events on Friday", GS-REQ-008, 8.2.8).
    public int RemainingCount => TotalCount - EventIds.Count;
}

// Groups the events of a period by day for the period view (GS-REQ-008, 8.2.6, 8.2.8, 8.2.9).
// Pure functions over EventListingKeyDto, so the rules are testable without a database.
public static class PeriodDaySelector
{
    // Cards shown under each day heading before "More N events" (GS-REQ-008, 8.2.8).
    public const int EventsPerDay = 8;

    // Days in chronological order; days without events are not returned (8.2.9). Within a day the events are
    // ordered by start time, events without a known time last (8.2.8); at most perDay ids per day.
    public static IReadOnlyList<PeriodDaySelection> SelectDays(IEnumerable<EventListingKeyDto> events, int perDay = EventsPerDay) =>
        events
            .GroupBy(e => e.Date.Date)
            .OrderBy(group => group.Key)
            .Select(group => new PeriodDaySelection(
                group.Key,
                group.Count(),
                OrderWithinDay(group).Take(perDay).Select(e => e.Id).ToList()))
            .ToList();

    // The events of one day after the first skip, in the same order as SelectDays ("More N events", 8.2.8).
    public static IReadOnlyList<int> SelectRemaining(IEnumerable<EventListingKeyDto> events, DateTime day, int skip = EventsPerDay) =>
        OrderWithinDay(events.Where(e => e.Date.Date == day.Date))
            .Skip(skip)
            .Select(e => e.Id)
            .ToList();

    // By start time, events without a known time last; ties by name, then id, so the order is stable between
    // the first page and "More N events". Same rule as HomeEventOrdering.ByStartTime.
    private static IEnumerable<EventListingKeyDto> OrderWithinDay(IEnumerable<EventListingKeyDto> events) =>
        events
            .OrderBy(e => EventDateTimeHelper.HasKnownTime(e.Date, e.StartTime) ? 0 : 1)
            .ThenBy(e => e.StartTime ?? e.Date.TimeOfDay)
            .ThenBy(e => e.Name, StringComparer.CurrentCulture)
            .ThenBy(e => e.Id);
}
