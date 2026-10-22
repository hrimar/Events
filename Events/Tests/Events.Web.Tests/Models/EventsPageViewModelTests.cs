using Events.Models.Enums;
using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class EventsPageViewModelTests
{
    // HasActiveFilters (GS-BUG-004: "Filtered results" only for real filters)

    [Fact]
    public void HasActiveFilters_DefaultStartDateAndSorting_IsFalse()
    {
        var model = new EventsPageViewModel
        {
            FromDate = new DateTime(2026, 9, 25),
            HasCustomFromDate = false,
            SortBy = "name"
        };

        Assert.False(model.HasActiveFilters);
    }

    [Fact]
    public void HasActiveFilters_CustomStartDate_IsTrue()
    {
        var model = new EventsPageViewModel { FromDate = new DateTime(2026, 10, 3), HasCustomFromDate = true };

        Assert.True(model.HasActiveFilters);
    }

    [Fact]
    public void HasActiveFilters_Category_IsTrue()
    {
        var model = new EventsPageViewModel { CurrentCategory = "Music" };

        Assert.True(model.HasActiveFilters);
    }

    // PopularTagsByCategory

    [Fact]
    public void PopularTagsByCategory_ExcludesTagsWithoutDisplayableCategory()
    {
        var model = new EventsPageViewModel
        {
            PopularTags =
            [
                new TagViewModel { Name = "Jazz", Category = EventCategory.Music },
                new TagViewModel { Name = "Misc", Category = EventCategory.Undefined },
                new TagViewModel { Name = "NoCategory", Category = null }
            ]
        };

        var group = Assert.Single(model.PopularTagsByCategory);
        Assert.Equal(EventCategory.Music, group.Key);
    }
}
