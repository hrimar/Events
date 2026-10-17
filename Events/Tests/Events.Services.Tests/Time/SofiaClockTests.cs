using Events.Services.Time;

namespace Events.Services.Tests.Time;

public class SofiaClockTests
{
    // Minimal TimeProvider returning a fixed UTC instant.
    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }

    private static SofiaClock CreateClock(DateTime utcNow) =>
        new(new FixedTimeProvider(new DateTimeOffset(utcNow, TimeSpan.Zero)));

    [Fact]
    public void Today_SummerTimeAfterUtcMidnightOffset_IsAlreadyNextSofiaDay()
    {
        // 22:30 UTC on 25.09 = 01:30 EEST (UTC+3) on 26.09 in Sofia.
        var clock = CreateClock(new DateTime(2026, 9, 25, 22, 30, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 9, 26), clock.Today);
    }

    [Fact]
    public void Today_WinterTimeBeforeSofiaMidnight_IsStillSameDay()
    {
        // 21:30 UTC on 15.01 = 23:30 EET (UTC+2) on 15.01 in Sofia.
        var clock = CreateClock(new DateTime(2026, 1, 15, 21, 30, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 1, 15), clock.Today);
    }

    [Fact]
    public void Today_WinterTimeAfterSofiaMidnight_IsNextDay()
    {
        // 22:30 UTC on 15.01 = 00:30 EET on 16.01 in Sofia.
        var clock = CreateClock(new DateTime(2026, 1, 15, 22, 30, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 1, 16), clock.Today);
    }

    [Fact]
    public void Today_CrawlerJobScheduleAt0100Utc_MatchesUtcCalendarDay()
    {
        // The crawler Container Apps Job runs at 01:00 UTC (03:00/04:00 Sofia):
        // the Sofia day and the UTC day are the same at that moment.
        var clock = CreateClock(new DateTime(2026, 7, 10, 1, 0, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 7, 10), clock.Today);
    }

    [Fact]
    public void Now_ReturnsSofiaWallClockWithUnspecifiedKind()
    {
        var clock = CreateClock(new DateTime(2026, 9, 25, 16, 0, 0, DateTimeKind.Utc));

        var now = clock.Now;

        Assert.Equal(new DateTime(2026, 9, 25, 19, 0, 0), now);
        Assert.Equal(DateTimeKind.Unspecified, now.Kind);
    }

    [Fact]
    public void Now_AcrossDaylightSavingSwitch_UsesCorrectOffset()
    {
        // DST ends on 25.10.2026 at 04:00 EEST -> 03:00 EET (01:00 UTC).
        var beforeSwitch = CreateClock(new DateTime(2026, 10, 25, 0, 30, 0, DateTimeKind.Utc));
        var afterSwitch = CreateClock(new DateTime(2026, 10, 25, 1, 30, 0, DateTimeKind.Utc));

        Assert.Equal(new DateTime(2026, 10, 25, 3, 30, 0), beforeSwitch.Now);
        Assert.Equal(new DateTime(2026, 10, 25, 3, 30, 0), afterSwitch.Now);
    }
}
