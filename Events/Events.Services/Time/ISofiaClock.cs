namespace Events.Services.Time;

// Provides the current date and time in Europe/Sofia, independent of the server time zone
// (Azure App Service and the crawler container both run in UTC).
// Event dates are stored as Sofia wall-clock values without offset, so every "now" / "today"
// comparison against Event.Date must go through this clock - never DateTime.Now/Today/UtcNow.
public interface ISofiaClock
{
    // Current Sofia wall-clock date and time (DateTimeKind.Unspecified, same as stored event dates).
    DateTime Now { get; }

    // Current calendar day in Sofia (midnight, DateTimeKind.Unspecified).
    DateTime Today { get; }
}
