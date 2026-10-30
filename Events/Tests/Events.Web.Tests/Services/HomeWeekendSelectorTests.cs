using Events.Web.Models;
using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class HomeWeekendSelectorTests
{
    // Saturday, 26.09.2026.
    private static readonly DateTime Saturday = new(2026, 9, 26);
    private static readonly DateTime Sunday = Saturday.AddDays(1);

    private static int _nextId;

    private static EventViewModel Event(DateTime day, string? time, params string[] tags) => new()
    {
        Id = ++_nextId,
        Name = $"Event {_nextId}",
        Date = day,
        StartTime = time is null ? null : TimeSpan.Parse(time),
        Tags = tags.ToList()
    };

    private static List<string?> Times(HomeWeekendCardsViewModel page) => page.Cards.Select(c => c.TimeText).ToList();

    private static readonly List<EventViewModel> Events = new()
    {
        Event(Saturday, "20:00"),
        Event(Saturday, "11:00", "For kids"),
        Event(Saturday, "18:00"),
        Event(Saturday, "12:00"),
        Event(Saturday, null, "Family-friendly"),
        Event(Sunday, "10:00")
    };

    [Fact]
    public void SelectPage_All_IsTheDayByTime_WithoutTimeLast()
    {
        var page = HomeWeekendSelector.SelectPage(Events, Saturday, HomeWeekendFilter.All, skip: 0);

        Assert.Equal(new[] { "11:00", "12:00", "18:00", "20:00", null }, Times(page));
        Assert.All(page.Cards, c => Assert.False(c.ShowDate));
    }

    [Fact]
    public void SelectPage_Morning_IsBeforeNoon_AndLeavesOutEventsWithoutTime()
    {
        var page = HomeWeekendSelector.SelectPage(Events, Saturday, HomeWeekendFilter.Morning, skip: 0);

        Assert.Equal(new[] { "11:00" }, Times(page));
    }

    [Fact]
    public void SelectPage_Evening_StartsAtSixPm()
    {
        var page = HomeWeekendSelector.SelectPage(Events, Saturday, HomeWeekendFilter.Evening, skip: 0);

        Assert.Equal(new[] { "18:00", "20:00" }, Times(page));
    }

    [Fact]
    public void SelectPage_Kids_MatchesEitherKidsTag()
    {
        var page = HomeWeekendSelector.SelectPage(Events, Saturday, HomeWeekendFilter.Kids, skip: 0);

        Assert.Equal(new[] { "11:00", null }, Times(page));
    }

    [Fact]
    public void SelectPage_PagesByEight()
    {
        var events = Enumerable.Range(0, 20).Select(i => Event(Sunday, $"{8 + i / 2:00}:{i % 2 * 30:00}")).ToList();

        var first = HomeWeekendSelector.SelectPage(events, Sunday, HomeWeekendFilter.All, skip: 0);
        var last = HomeWeekendSelector.SelectPage(events, Sunday, HomeWeekendFilter.All, skip: 16);

        Assert.Equal(HomeWeekendSelector.PageSize, first.Cards.Count);
        Assert.True(first.HasMore);
        Assert.Equal(4, last.Cards.Count);
        Assert.False(last.HasMore);
    }

    [Theory]
    [InlineData("morning", HomeWeekendFilter.Morning)]
    [InlineData("KIDS", HomeWeekendFilter.Kids)]
    [InlineData(null, HomeWeekendFilter.All)]
    [InlineData("nonsense", HomeWeekendFilter.All)]
    public void ParseFilter_UnknownMeansAll(string? value, HomeWeekendFilter expected)
    {
        Assert.Equal(expected, HomeWeekendSelector.ParseFilter(value));
    }
}
