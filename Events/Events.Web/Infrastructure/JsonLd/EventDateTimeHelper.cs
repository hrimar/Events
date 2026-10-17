using System.Globalization;
using Events.Services.Time;

namespace Events.Web.Infrastructure.JsonLd;

// Centralizes the "do we actually know this event's start time" check and the ISO 8601
// formatting used for schema.org startDate, so no other code (views, JSON-LD builders)
// has to re-derive the sentinel convention or guess a timezone offset on its own.
public static class EventDateTimeHelper
{
    // The crawler pipeline has no dedicated "time known" flag: when a source only
    // provides a date, StartTime/Date.TimeOfDay ends up at exactly midnight
    // (TimeSpan.Zero) as a sentinel for "unknown time", not a genuine midnight event.
    // This is the single source of truth for that convention - EventViewModel.HasKnownTime
    // and any JSON-LD builder must go through this method so they can never disagree.
    public static bool HasKnownTime(DateTime date, TimeSpan? startTime)
    {
        return startTime.HasValue ? startTime.Value != TimeSpan.Zero : date.TimeOfDay != TimeSpan.Zero;
    }

    // schema.org startDate: full ISO 8601 with the Sofia UTC offset when the time is
    // known, otherwise a date-only value - we never assert a fabricated 00:00:00 start time to Google.
    public static string ToIso8601StartDate(DateTime date, TimeSpan? startTime)
    {
        if (!HasKnownTime(date, startTime))
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var localDateTime = startTime.HasValue ? date.Date + startTime.Value : date;
        var offset = SofiaTimeZone.Instance.GetUtcOffset(localDateTime);

        return new DateTimeOffset(localDateTime, offset).ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
    }

    // An event is past from 00:00 on the calendar day after Date, in Europe/Sofia.
    // StartTime is ignored: a 19:00 show stays current through that whole Sofia day.
    // sofiaToday comes from ISofiaClock.Today, so this rule never reads the current time itself.
    public static bool IsPastEvent(DateTime date, DateTime sofiaToday)
    {
        return date.Date < sofiaToday.Date;
    }
}
