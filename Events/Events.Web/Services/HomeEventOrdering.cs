using Events.Web.Models;

namespace Events.Web.Services;

// Order of events within one day on the homepage ("Tonight", "This weekend", GS-REQ-007, 7.5.2 and 7.6.5).
public static class HomeEventOrdering
{
    // By start time; events without a known time go last, ties by name.
    public static IEnumerable<EventViewModel> ByStartTime(IEnumerable<EventViewModel> events) =>
        events
            .OrderBy(e => e.HasKnownTime ? 0 : 1)
            .ThenBy(GetStartTime)
            .ThenBy(e => e.Name, StringComparer.CurrentCulture);

    // Start time of day: StartTime, or the time part of Date when StartTime is not set.
    public static TimeSpan GetStartTime(EventViewModel eventModel) => eventModel.StartTime ?? eventModel.Date.TimeOfDay;
}
