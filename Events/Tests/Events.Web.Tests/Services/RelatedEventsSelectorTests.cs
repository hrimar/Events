using Events.Models.Entities;
using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class RelatedEventsSelectorTests
{
    private static readonly DateTime Friday = new(2026, 10, 30, 19, 0, 0);

    private static Event NewEvent(int id, string name, DateTime date) => new() { Id = id, Name = name, Date = date };

    [Fact]
    public void Select_SkipsTheCurrentEvent_AndItsOtherPerformanceOnTheSameDay()
    {
        var current = NewEvent(1, "Хамлет", Friday);
        var candidates = new[]
        {
            current,
            NewEvent(2, "хамлет ", Friday.AddHours(2)),
            NewEvent(3, "Хамлет", Friday.AddDays(1)),
            NewEvent(4, "Макбет", Friday)
        };

        var selected = RelatedEventsSelector.Select(candidates, current);

        Assert.Equal(new[] { 3, 4 }, selected.Select(e => e.Id));
    }

    [Fact]
    public void Select_KeepsOneOfTheRepeatsAmongTheCandidates_AndAtMostFour()
    {
        var current = NewEvent(1, "Хамлет", Friday);
        var candidates = new[]
        {
            NewEvent(2, "Джаз вечер", Friday),
            NewEvent(3, "Джаз вечер", Friday),
            NewEvent(4, "Опера", Friday),
            NewEvent(5, "Балет", Friday),
            NewEvent(6, "Рок", Friday),
            NewEvent(7, "Поп", Friday)
        };

        var selected = RelatedEventsSelector.Select(candidates, current);

        Assert.Equal(new[] { 2, 4, 5, 6 }, selected.Select(e => e.Id));
    }
}
