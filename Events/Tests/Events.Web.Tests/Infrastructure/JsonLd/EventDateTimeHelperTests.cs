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

    // IsPastEvent - "today" is passed in, so the tests are deterministic.

    private static readonly DateTime SofiaToday = new(2026, 9, 25);

    [Fact]
    public void IsPastEvent_Yesterday_ReturnsTrue()
    {
        Assert.True(EventDateTimeHelper.IsPastEvent(new DateTime(2026, 9, 24, 20, 0, 0), SofiaToday));
    }

    [Fact]
    public void IsPastEvent_TodayWithStartTimeAlreadyPassed_ReturnsFalse()
    {
        // A show that started at 10:00 stays current through the whole Sofia day.
        Assert.False(EventDateTimeHelper.IsPastEvent(new DateTime(2026, 9, 25, 10, 0, 0), SofiaToday));
    }

    [Fact]
    public void IsPastEvent_Tomorrow_ReturnsFalse()
    {
        Assert.False(EventDateTimeHelper.IsPastEvent(new DateTime(2026, 9, 26), SofiaToday));
    }
}
