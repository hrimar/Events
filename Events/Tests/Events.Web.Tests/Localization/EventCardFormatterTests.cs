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
}
