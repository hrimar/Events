using Events.Web.Models;

namespace Events.Web.Services;

// Order of events on the homepage: within one day ("Tonight", "This weekend", GS-REQ-007, 7.5.2 and 7.6.5)
// and across days ("Free this week", GS-REQ-007, 7.9.2).
public static class HomeEventOrdering
{
    // By start time; events without a known time go last, ties by name.
    public static IEnumerable<EventViewModel> ByStartTime(IEnumerable<EventViewModel> events) =>
        events
            .OrderBy(e => e.HasKnownTime ? 0 : 1)
            .ThenBy(GetStartTime)
            .ThenBy(e => e.Name, StringComparer.CurrentCulture);

    // By day, then by start time within the day (events without a known time last in their day).
    public static IEnumerable<EventViewModel> ByDateAndStartTime(IEnumerable<EventViewModel> events) =>
        events
            .OrderBy(e => e.Date.Date)
            .ThenBy(e => e.HasKnownTime ? 0 : 1)
            .ThenBy(GetStartTime)
            .ThenBy(e => e.Name, StringComparer.CurrentCulture);

    // Start time of day: StartTime, or the time part of Date when StartTime is not set.
    public static TimeSpan GetStartTime(EventViewModel eventModel) => eventModel.StartTime ?? eventModel.Date.TimeOfDay;
}
