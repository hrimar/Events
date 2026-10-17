namespace Events.Services.Time;

// ISofiaClock built on top of TimeProvider, so tests can substitute a fixed instant.
public sealed class SofiaClock : ISofiaClock
{
    private readonly TimeProvider _timeProvider;

    public SofiaClock(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    // DateTimeOffset.DateTime has DateTimeKind.Unspecified - matches how event dates are stored.
    public DateTime Now => TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), SofiaTimeZone.Instance).DateTime;

    public DateTime Today => Now.Date;
}
