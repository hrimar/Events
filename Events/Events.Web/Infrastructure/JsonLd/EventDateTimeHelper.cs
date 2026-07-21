using System.Globalization;

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

    private static readonly TimeZoneInfo SofiaTimeZone = ResolveSofiaTimeZone();

    // schema.org startDate: full ISO 8601 with the Sofia UTC offset when the time is
    // known, otherwise a date-only value - we never assert a fabricated 00:00:00 start
    // time to Google.
    public static string ToIso8601StartDate(DateTime date, TimeSpan? startTime)
    {
        if (!HasKnownTime(date, startTime))
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var localDateTime = startTime.HasValue ? date.Date + startTime.Value : date;
        var offset = SofiaTimeZone.GetUtcOffset(localDateTime);

        return new DateTimeOffset(localDateTime, offset).ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
    }

    // Whether the event has already happened, as of the current moment in Sofia time.
    // When the time is known, compares the exact Date+StartTime moment. When only a date
    // is known, the event is considered past only once that whole calendar day has
    // elapsed (from the start of the next day) - not from midnight on the event's own
    // day, which would otherwise mark it "past" before it has even happened.
    public static bool IsPastEvent(DateTime date, TimeSpan? startTime)
    {
        var nowInSofia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SofiaTimeZone);

        if (!HasKnownTime(date, startTime))
            return date.Date.AddDays(1) <= nowInSofia;

        var eventMoment = startTime.HasValue ? date.Date + startTime.Value : date;
        return eventMoment < nowInSofia;
    }

    private static TimeZoneInfo ResolveSofiaTimeZone()
    {
        try
        {
            // IANA id - works cross-platform on .NET 8 (Windows, Linux with ICU).
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia");
        }
        catch (TimeZoneNotFoundException)
        {
            // Windows-only fallback id, in case ICU/IANA data isn't available.
            return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        }
    }
}
