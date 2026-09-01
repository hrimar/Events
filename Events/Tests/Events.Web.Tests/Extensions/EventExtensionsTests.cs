using Events.Models.Entities;
using Events.Web.Extensions;

namespace Events.Web.Tests.Extensions;

public class EventExtensionsTests
{
    private static Event CreateEvent() => new()
    {
        Id = 1,
        Name = "Concert",
        Location = "NDK",
        Date = new DateTime(2026, 6, 15)
    };

    // ToSuggestionDto

    [Fact]
    public void ToSuggestionDto_MapsCoreFieldsAndFormatsDateAsDayMonthYear()
    {
        var eventEntity = CreateEvent();
        eventEntity.Category = new Category { Name = "Music" };

        var result = eventEntity.ToSuggestionDto();

        Assert.Equal(1, result.Id);
        Assert.Equal("Concert", result.Name);
        Assert.Equal("Music", result.Category);
        Assert.Equal("NDK", result.Location);
        Assert.Equal("15.06.2026", result.Date);
    }

    [Fact]
    public void ToSuggestionDto_NoCategoryAssigned_CategoryIsNull()
    {
        var result = CreateEvent().ToSuggestionDto();

        Assert.Null(result.Category);
    }

    [Fact]
    public void ToSuggestionDto_MoreThanThreeTags_TakesOnlyTheFirstThree()
    {
        var eventEntity = CreateEvent();
        eventEntity.EventTags =
        [
            new EventTag { Tag = new Tag { Name = "Live" } },
            new EventTag { Tag = new Tag { Name = "Rock" } },
            new EventTag { Tag = new Tag { Name = "Outdoor" } },
            new EventTag { Tag = new Tag { Name = "Extra" } } // beyond the 3-tag cap
        ];

        var result = eventEntity.ToSuggestionDto();

        Assert.Equal(["Live", "Rock", "Outdoor"], result.Tags);
    }

    // .Take(3) runs BEFORE the null-Tag filter, not after: an unresolved tag within the
    // first 3 reduces the shown count instead of being skipped in favor of a later one.
    [Fact]
    public void ToSuggestionDto_NullTagWithinFirstThree_ReducesResultCountInsteadOfBeingSkipped()
    {
        var eventEntity = CreateEvent();
        eventEntity.EventTags =
        [
            new EventTag { Tag = new Tag { Name = "Live" } },
            new EventTag { Tag = null! }, // crawler data can leave this unresolved
            new EventTag { Tag = new Tag { Name = "Rock" } },
            new EventTag { Tag = new Tag { Name = "Outdoor" } } // never reached - Take(3) already ran
        ];

        var result = eventEntity.ToSuggestionDto();

        Assert.Equal(["Live", "Rock"], result.Tags);
    }

    [Fact]
    public void ToSuggestionDto_NoTags_ReturnsEmptyArrayNotNull()
    {
        var result = CreateEvent().ToSuggestionDto();

        Assert.Empty(result.Tags);
    }

    // ToSearchSuggestionDto

    [Fact]
    public void ToSearchSuggestionDto_SetsIsTagFalseAndTagNameNull()
    {
        var result = CreateEvent().ToSearchSuggestionDto();

        Assert.False(result.IsTag);
        Assert.Null(result.TagName);
    }
}
