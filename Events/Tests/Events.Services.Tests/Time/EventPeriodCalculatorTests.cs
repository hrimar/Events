using Events.Models.Enums;
using Events.Services.Time;

namespace Events.Services.Tests.Time;

public class EventPeriodCalculatorTests
{
    // 25.09.2026 is a Friday.
    private static readonly DateTime Friday = new(2026, 9, 25);

    [Theory]
    [InlineData("today", EventPeriod.Today)]
    [InlineData("TOMORROW", EventPeriod.Tomorrow)]
    [InlineData(" weekend ", EventPeriod.Weekend)]
    [InlineData("next7", EventPeriod.Next7)]
    public void TryParse_KnownValue_ReturnsPeriod(string value, EventPeriod expected)
    {
        Assert.True(EventPeriodCalculator.TryParse(value, out var period));
        Assert.Equal(expected, period);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("next30")]
    [InlineData("4")]
    public void TryParse_UnknownValue_ReturnsFalse(string? value)
    {
        Assert.False(EventPeriodCalculator.TryParse(value, out _));
    }

    [Fact]
    public void ToQueryValue_RoundTripsWithTryParse()
    {
        foreach (var period in Enum.GetValues<EventPeriod>())
        {
            Assert.True(EventPeriodCalculator.TryParse(EventPeriodCalculator.ToQueryValue(period), out var parsed));
            Assert.Equal(period, parsed);
        }
    }

    [Fact]
    public void GetRange_Today_IsSingleDay()
    {
        Assert.Equal((Friday, Friday), EventPeriodCalculator.GetRange(EventPeriod.Today, Friday.AddHours(23)));
    }

    [Fact]
    public void GetRange_Tomorrow_IsNextDay()
    {
        Assert.Equal((Friday.AddDays(1), Friday.AddDays(1)), EventPeriodCalculator.GetRange(EventPeriod.Tomorrow, Friday));
    }

    [Fact]
    public void GetRange_Next7_IsTodayPlusSixDays()
    {
        Assert.Equal((Friday, new DateTime(2026, 10, 1)), EventPeriodCalculator.GetRange(EventPeriod.Next7, Friday));
    }

    [Theory]
    [InlineData(21)] // Monday
    [InlineData(23)] // Wednesday
    [InlineData(25)] // Friday
    public void GetWeekendRange_Weekday_IsComingSaturdayAndSunday(int day)
    {
        var range = EventPeriodCalculator.GetWeekendRange(new DateTime(2026, 9, day));

        Assert.Equal((new DateTime(2026, 9, 26), new DateTime(2026, 9, 27)), range);
    }

    [Fact]
    public void GetWeekendRange_Saturday_IsTodayAndTomorrow()
    {
        var saturday = new DateTime(2026, 9, 26);

        Assert.Equal((saturday, saturday.AddDays(1)), EventPeriodCalculator.GetWeekendRange(saturday));
    }

    [Fact]
    public void GetWeekendRange_Sunday_IsOnlyToday()
    {
        var sunday = new DateTime(2026, 9, 27);

        Assert.Equal((sunday, sunday), EventPeriodCalculator.GetWeekendRange(sunday));
    }

    [Fact]
    public void GetFallbackRange_IsThirtyDaysFromToday()
    {
        Assert.Equal((Friday, Friday.AddDays(29)), EventPeriodCalculator.GetFallbackRange(Friday));
    }
}
