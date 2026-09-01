using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Tests.Infrastructure.JsonLd;

public class EventDateTimeHelperTests
{
    // HasKnownTime

    [Fact]
    public void HasKnownTime_StartTimeIsNonZero_ReturnsTrue()
    {
        Assert.True(EventDateTimeHelper.HasKnownTime(new DateTime(2026, 6, 1), TimeSpan.FromHours(19)));
    }

    [Fact]
    public void HasKnownTime_StartTimeIsExactlyMidnight_TreatedAsUnknownSentinel()
    {
        Assert.False(EventDateTimeHelper.HasKnownTime(new DateTime(2026, 6, 1), TimeSpan.Zero));
    }

    [Fact]
    public void HasKnownTime_NoStartTime_FallsBackToDateTimeOfDay()
    {
        Assert.True(EventDateTimeHelper.HasKnownTime(new DateTime(2026, 6, 1, 19, 0, 0), null));
        Assert.False(EventDateTimeHelper.HasKnownTime(new DateTime(2026, 6, 1, 0, 0, 0), null));
    }

    // ToIso8601StartDate

    [Fact]
    public void ToIso8601StartDate_UnknownTime_ReturnsDateOnlyFormat()
    {
        var result = EventDateTimeHelper.ToIso8601StartDate(new DateTime(2026, 6, 1), null);

        Assert.Equal("2026-06-01", result);
    }

    [Fact]
    public void ToIso8601StartDate_KnownTime_ReturnsFullIso8601WithSofiaOffset()
    {
        var result = EventDateTimeHelper.ToIso8601StartDate(new DateTime(2026, 6, 1), TimeSpan.FromHours(19));

        // Sofia is UTC+3 in June (EEST, daylight saving) - the offset must be reflected, not fabricated as +00:00.
        Assert.Equal("2026-06-01T19:00:00+03:00", result);
    }

    [Fact]
    public void ToIso8601StartDate_WinterDate_UsesStandardTimeOffset()
    {
        var result = EventDateTimeHelper.ToIso8601StartDate(new DateTime(2026, 1, 15), TimeSpan.FromHours(19));

        // Sofia is UTC+2 in January (EET, standard time).
        Assert.Equal("2026-01-15T19:00:00+02:00", result);
    }

    // IsPastEvent - uses dates safely far from "today" so the test stays deterministic
    // regardless of what day it actually runs on.

    [Fact]
    public void IsPastEvent_DateInThePast_ReturnsTrue()
    {
        Assert.True(EventDateTimeHelper.IsPastEvent(new DateTime(2020, 1, 1)));
    }

    [Fact]
    public void IsPastEvent_DateInTheFuture_ReturnsFalse()
    {
        Assert.False(EventDateTimeHelper.IsPastEvent(new DateTime(2030, 1, 1)));
    }
}
