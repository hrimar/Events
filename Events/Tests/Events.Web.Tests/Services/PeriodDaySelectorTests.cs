using Events.Models.DTOs;
using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class PeriodDaySelectorTests
{
    private static readonly DateTime Friday = new(2026, 9, 25);

    private static EventListingKeyDto Event(int id, DateTime day, string? time, string name = "Event") =>
        new(id, day, time is null ? null : TimeSpan.Parse(time), name);

    [Fact]
    public void SelectDays_GroupsByDayInDateOrder_WithoutEmptyDays()
    {
        var events = new[]
        {
            Event(1, Friday.AddDays(2), "20:00"),
            Event(2, Friday, "19:00"),
            Event(3, Friday, "10:00")
        };

        var days = PeriodDaySelector.SelectDays(events);

        Assert.Equal(new[] { Friday, Friday.AddDays(2) }, days.Select(d => d.Day));
        Assert.Equal(new[] { 2, 1 }, days.Select(d => d.TotalCount));
    }

    [Fact]
    public void SelectDays_OrdersByTime_EventsWithoutTimeLast()
    {
        var events = new[]
        {
            Event(1, Friday, null),
            Event(2, Friday, "21:30"),
            Event(3, Friday, "00:00"),
            Event(4, Friday, "10:00")
        };

        var day = Assert.Single(PeriodDaySelector.SelectDays(events));

        // 00:00 is "no known time" (EventDateTimeHelper.HasKnownTime), so it goes last with the null time
        Assert.Equal(new[] { 4, 2, 1, 3 }, day.EventIds);
    }

    [Fact]
    public void SelectDays_KeepsAtMostPerDay_AndCountsTheRest()
    {
        var events = Enumerable.Range(1, 11).Select(i => Event(i, Friday, $"{8 + i}:00")).ToList();

        var day = Assert.Single(PeriodDaySelector.SelectDays(events));

        Assert.Equal(11, day.TotalCount);
        Assert.Equal(Enumerable.Range(1, 8), day.EventIds);
        Assert.Equal(3, day.RemainingCount);
    }

    [Fact]
    public void SelectRemaining_ContinuesTheOrderOfTheFirstPage()
    {
        var events = Enumerable.Range(1, 10)
            .Select(i => Event(i, Friday, "20:00", $"Name {i:00}"))
            .Append(Event(99, Friday.AddDays(1), "20:00"))
            .ToList();

        var remaining = PeriodDaySelector.SelectRemaining(events, Friday);

        Assert.Equal(new[] { 9, 10 }, remaining);
    }
}
