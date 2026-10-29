using System.Globalization;
using Events.Web.Localization;
using Events.Web.Models;

namespace Events.Web.Tests.Localization;

public class EventCardFormatterTests
{
    private static readonly CultureInfo Bulgarian = new("bg-BG");
    private static readonly CultureInfo English = new("en-US");

    // 25.09.2026 is a Friday
    private static readonly DateTime Friday = new(2026, 9, 25);

    [Fact]
    public void FormatBadge_WithDateAndTime_ShowsDayDateAndTime()
    {
        var card = new EventCardViewModel { Date = Friday, TimeText = "19:00" };

        Assert.Equal("Пт 25.09 · 19:00", EventCardFormatter.FormatBadge(card, Bulgarian));
        Assert.Equal("Fri 25/09 · 19:00", EventCardFormatter.FormatBadge(card, English));
    }

    [Fact]
    public void FormatBadge_WithDateAndUnknownTime_ShowsOnlyTheDate()
    {
        var card = new EventCardViewModel { Date = Friday };

        Assert.Equal("Пт 25.09", EventCardFormatter.FormatBadge(card, Bulgarian));
    }

    [Fact]
    public void FormatBadge_PastEvent_ShowsDateAndTimeLikeAnyOtherEvent()
    {
        var card = new EventCardViewModel { Date = Friday, TimeText = "19:00", IsPast = true };

        Assert.Equal("Пт 25.09 · 19:00", EventCardFormatter.FormatBadge(card, Bulgarian));
    }

    [Fact]
    public void FormatBadge_UnderDayHeading_ShowsOnlyTheTime()
    {
        var card = new EventCardViewModel { Date = Friday, TimeText = "19:00", ShowDate = false };

        Assert.Equal("19:00", EventCardFormatter.FormatBadge(card, Bulgarian));
    }

    [Fact]
    public void FormatBadge_UnderDayHeadingWithUnknownTime_ReturnsNull()
    {
        var card = new EventCardViewModel { Date = Friday, ShowDate = false };

        Assert.Null(EventCardFormatter.FormatBadge(card, Bulgarian));
    }

    [Theory]
    [InlineData(2026, 9, 26, "Сб 26.09")]
    [InlineData(2026, 9, 27, "Нд 27.09")]
    [InlineData(2026, 9, 28, "Пн 28.09")]
    public void FormatShortDate_Bulgarian_CapitalizesTheDayName(int year, int month, int day, string expected)
    {
        Assert.Equal(expected, EventCardFormatter.FormatShortDate(new DateTime(year, month, day), Bulgarian));
    }

    [Fact]
    public void FormatWeekdayAndDate_MatchesTheHeroEyebrow()
    {
        var friday = new DateTime(2026, 9, 25);

        Assert.Equal("Петък, 25 септември", EventCardFormatter.FormatWeekdayAndDate(friday, Bulgarian));
        Assert.Equal("Friday, 25 September", EventCardFormatter.FormatWeekdayAndDate(friday, English));
    }

    [Fact]
    public void FormatDayAndMonthName_MatchesTheCarouselMetaLine()
    {
        var saturday = new DateTime(2026, 9, 26);

        Assert.Equal("Сб 26 септември", EventCardFormatter.FormatDayAndMonthName(saturday, Bulgarian));
        Assert.Equal("Sat 26 September", EventCardFormatter.FormatDayAndMonthName(saturday, English));
    }

    [Fact]
    public void FormatWeekdayAndShortMonth_MatchesTheWeekendTabs()
    {
        var saturday = new DateTime(2026, 9, 26);

        Assert.Equal("Събота, 26 сеп", EventCardFormatter.FormatWeekdayAndShortMonth(saturday, Bulgarian));
        Assert.Equal("Saturday, 26 Sep", EventCardFormatter.FormatWeekdayAndShortMonth(saturday, English));
    }

    [Theory]
    [InlineData(2026, 9, 26, 2026, 9, 27, "26–27 септември")]
    [InlineData(2026, 10, 31, 2026, 11, 1, "31 октомври – 1 ноември")]
    [InlineData(2026, 9, 27, 2026, 9, 27, "27 септември")]
    public void FormatDayRange_SameMonthOtherMonthAndSingleDay(int y1, int m1, int d1, int y2, int m2, int d2, string expected)
    {
        Assert.Equal(expected, EventCardFormatter.FormatDayRange(new DateTime(y1, m1, d1), new DateTime(y2, m2, d2), Bulgarian));
    }

    [Fact]
    public void FormatWeekdayAndDateRange_TwoDays_SecondDayKeepsCultureCase()
    {
        var thursday = new DateTime(2026, 10, 1);

        Assert.Equal("Петък, 25 септември – четвъртък, 1 октомври",
            EventCardFormatter.FormatWeekdayAndDateRange(Friday, thursday, Bulgarian));
        Assert.Equal("Friday, 25 September – Thursday, 1 October",
            EventCardFormatter.FormatWeekdayAndDateRange(Friday, thursday, English));
    }

    [Fact]
    public void FormatWeekdayAndDateRange_OneDay_ShowsOnlyThatDay()
    {
        Assert.Equal("Петък, 25 септември", EventCardFormatter.FormatWeekdayAndDateRange(Friday, Friday, Bulgarian));
    }
}
