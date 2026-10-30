using Events.Web.Models;
using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class HomeTonightRowSelectorTests
{
    // Friday, 25.09.2026.
    private static readonly DateTime Today = new(2026, 9, 25);
    private static readonly DateTime Tomorrow = Today.AddDays(1);

    private static int _nextId;

    private static EventViewModel Event(DateTime day, string? time, string name = "Event") => new()
    {
        Id = ++_nextId,
        Name = name,
        Date = day,
        StartTime = time is null ? null : TimeSpan.Parse(time)
    };

    private static List<EventViewModel> TodayEvents(params string[] times) => times.Select(t => Event(Today, t)).ToList();

    [Fact]
    public void Select_BeforeFourPm_IsTodayWithRemainingEventsByTime()
    {
        var events = TodayEvents("21:00", "19:00", "20:00", "18:00", "22:00");

        var result = HomeTonightRowSelector.Select(events, Today.AddHours(15));

        Assert.NotNull(result);
        Assert.Equal(HomeTonightDay.Today, result.Day);
        Assert.Equal(new[] { "18:00", "19:00", "20:00", "21:00", "22:00" }, result.Cards.Select(c => c.TimeText));
    }

    [Fact]
    public void Select_FromFourPm_IsTonight()
    {
        var result = HomeTonightRowSelector.Select(TodayEvents("18:00", "19:00", "20:00", "21:00", "22:00"), Today.AddHours(16));

        Assert.Equal(HomeTonightDay.Tonight, result!.Day);
    }

    [Fact]
    public void Select_StartedEventsAreLeftOut_AndEventsWithoutTimeGoLast()
    {
        var events = TodayEvents("10:00", "17:00", "18:00", "19:00", "20:00", "21:00");
        events.Add(Event(Today, time: null, name: "No time"));

        var result = HomeTonightRowSelector.Select(events, Today.AddHours(16).AddMinutes(30));

        Assert.Equal(new[] { "17:00", "18:00", "19:00", "20:00", "21:00", null }, result!.Cards.Select(c => c.TimeText));
        Assert.Equal(7, result.TotalCount);
    }

    [Fact]
    public void Select_LateEveningWithFewEventsLeft_ShowsTomorrow()
    {
        // Criterion 6: at 23:30 "Tonight" shows tomorrow's events under "Tomorrow".
        var events = TodayEvents("19:00", "20:00");
        events.Add(Event(Tomorrow, "11:00"));
        events.Add(Event(Tomorrow, "10:00"));

        var result = HomeTonightRowSelector.Select(events, Today.AddHours(23).AddMinutes(30));

        Assert.Equal(HomeTonightDay.Tomorrow, result!.Day);
        Assert.Equal(Tomorrow, result.Date);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(new[] { "10:00", "11:00" }, result.Cards.Select(c => c.TimeText));
    }

    [Fact]
    public void Select_FewEventsLeftAndNothingTomorrow_KeepsToday()
    {
        var result = HomeTonightRowSelector.Select(TodayEvents("20:00", "21:00"), Today.AddHours(19));

        Assert.Equal(HomeTonightDay.Tonight, result!.Day);
        Assert.Equal(2, result.Cards.Count);
    }

    [Fact]
    public void Select_NothingLeft_ReturnsNull()
    {
        Assert.Null(HomeTonightRowSelector.Select(TodayEvents("10:00"), Today.AddHours(22)));
    }

    [Fact]
    public void Select_ShowsAtMostTwentyCards_WithTimeOnlyBadges()
    {
        var events = Enumerable.Range(0, 30).Select(i => Event(Today, "20:00", $"Event {i:00}")).ToList();

        var result = HomeTonightRowSelector.Select(events, Today.AddHours(12));

        Assert.Equal(HomeTonightRowSelector.MaxCards, result!.Cards.Count);
        Assert.All(result.Cards, c => Assert.False(c.ShowDate));
    }
}
