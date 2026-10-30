using Events.Web.Models;
using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class HomeEventOrderingTests
{
    private static readonly DateTime Friday = new(2026, 9, 25);

    private static EventViewModel Event(string name, DateTime day, string? time) => new()
    {
        Name = name,
        Date = day,
        StartTime = time is null ? null : TimeSpan.Parse(time)
    };

    [Fact]
    public void ByDateAndStartTime_OrdersByDayThenTime_WithoutTimeLastInItsDay()
    {
        var events = new[]
        {
            Event("Saturday morning", Friday.AddDays(1), "10:00"),
            Event("Friday no time", Friday, null),
            Event("Friday evening", Friday, "21:30"),
            Event("Friday afternoon", Friday, "15:00")
        };

        var ordered = HomeEventOrdering.ByDateAndStartTime(events).Select(e => e.Name);

        Assert.Equal(new[] { "Friday afternoon", "Friday evening", "Friday no time", "Saturday morning" }, ordered);
    }
}
