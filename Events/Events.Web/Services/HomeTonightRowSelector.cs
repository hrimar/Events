using Events.Web.Models;

namespace Events.Web.Services;

// Picks the day and the events of the homepage "Tonight" row (GS-REQ-007, 7.5) for a given Sofia "now".
// A pure function, so the time-dependent rules (16:00, events already started, switch to tomorrow) are testable.
public static class HomeTonightRowSelector
{
    // At most 20 cards in the row (GS-REQ-007, 7.5.3).
    public const int MaxCards = 20;

    // With fewer of today's events still ahead, the row shows tomorrow (GS-REQ-007, 7.5.5).
    public const int MinRemainingTodayEvents = 5;

    // From this time the heading is "Tonight" instead of "Today" (GS-REQ-007, 7.5.1).
    public static readonly TimeSpan EveningStart = TimeSpan.FromHours(16);

    // events: today's and tomorrow's events (other days are ignored); null when neither day has anything left.
    public static HomeTonightViewModel? Select(IEnumerable<EventViewModel> events, DateTime now)
    {
        var today = now.Date;
        var tomorrow = today.AddDays(1);
        var eventList = events.ToList();

        var todayEvents = eventList.Where(e => e.Date.Date == today).ToList();
        var tomorrowEvents = HomeEventOrdering.ByStartTime(eventList.Where(e => e.Date.Date == tomorrow)).ToList();

        // Events that have already started stay active in the lists, but are not shown here (GS-REQ-007, 7.5.2).
        var remainingTodayEvents = HomeEventOrdering
            .ByStartTime(todayEvents.Where(e => !e.HasKnownTime || HomeEventOrdering.GetStartTime(e) > now.TimeOfDay))
            .ToList();

        if (remainingTodayEvents.Count < MinRemainingTodayEvents && tomorrowEvents.Count > 0)
            return Create(HomeTonightDay.Tomorrow, tomorrow, tomorrowEvents.Count, tomorrowEvents);

        if (remainingTodayEvents.Count == 0)
            return null;

        var day = now.TimeOfDay >= EveningStart ? HomeTonightDay.Tonight : HomeTonightDay.Today;
        return Create(day, today, todayEvents.Count, remainingTodayEvents);
    }

    private static HomeTonightViewModel Create(HomeTonightDay day, DateTime date, int totalCount, IEnumerable<EventViewModel> events) =>
        new()
        {
            Day = day,
            Date = date,
            TotalCount = totalCount,

            // One day only, so the card badges show just the time (GS-REQ-006, 6.5.4).
            Cards = EventCardViewModel.FromEvents(events.Take(MaxCards), showDate: false)
        };
}
