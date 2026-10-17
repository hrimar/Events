namespace Events.Services.Time;

// The Europe/Sofia time zone as a process-wide constant (like TimeZoneInfo.Utc).
// Static on purpose: it holds no state and is used for pure conversions that do not depend
// on the current moment (e.g. the UTC offset of a stored event time in JSON-LD).
// Anything that needs "now" or "today" must use ISofiaClock instead.
public static class SofiaTimeZone
{
    private const string IanaId = "Europe/Sofia";
    private const string WindowsId = "FLE Standard Time";

    public static TimeZoneInfo Instance { get; } = Resolve();

    // IANA id works cross-platform on .NET 8 (Linux containers, Windows with ICU);
    // the Windows id is a fallback for hosts without IANA time zone data.
    private static TimeZoneInfo Resolve() =>
        TimeZoneInfo.TryFindSystemTimeZoneById(IanaId, out var timeZone)
            ? timeZone
            : TimeZoneInfo.FindSystemTimeZoneById(WindowsId);
}
