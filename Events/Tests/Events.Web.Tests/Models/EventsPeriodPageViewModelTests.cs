using Events.Models.Enums;
using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class EventsPeriodPageViewModelTests
{
    private static EventsPeriodPageViewModel Model(EventPeriod? period) => new()
    {
        Period = period,
        FromDate = new DateTime(2026, 10, 30),
        ToDate = new DateTime(2026, 10, 30),
        CurrentCategory = "Music",
        SelectedSubCategory = "Jazz",
        IsFreeFilter = true,
        SelectedTags = new List<string> { "Blues" },
        IsKidsFilter = true
    };

    [Fact]
    public void RouteWithPeriod_KeepsTheFilters_AndDropsExplicitDates()
    {
        var route = Model(period: null).RouteWithPeriod(EventPeriod.Weekend);

        Assert.Equal("weekend", route["period"]);
        Assert.False(route.ContainsKey("fromDate"));
        Assert.False(route.ContainsKey("toDate"));
        Assert.Equal("Music", route["category"]);
        Assert.Equal("Jazz", route["subCategory"]);
        Assert.Equal("true", route["free"]);
        Assert.Equal("Blues", route["tags"]);
        Assert.Equal("true", route["kids"]);
    }

    [Fact]
    public void FilterRoute_WithoutPeriod_UsesTheExplicitDates()
    {
        var route = Model(period: null).FilterRoute();

        Assert.Equal("2026-10-30", route["fromDate"]);
        Assert.Equal("2026-10-30", route["toDate"]);
        Assert.False(route.ContainsKey("period"));
    }

    [Fact]
    public void RouteWith_NewCategory_ClearsTheSubcategory_AndKeepsThePeriod()
    {
        var route = Model(EventPeriod.Next7).RouteWith("category", "Theatre");

        Assert.Equal("Theatre", route["category"]);
        Assert.False(route.ContainsKey("subCategory"));
        Assert.Equal("next7", route["period"]);
    }

    [Fact]
    public void RouteWith_NullValue_RemovesOnlyThatFilter()
    {
        var route = Model(EventPeriod.Next7).RouteWith("free", null);

        Assert.False(route.ContainsKey("free"));
        Assert.Equal("Jazz", route["subCategory"]);
        Assert.Equal("true", route["kids"]);
    }

    [Fact]
    public void RouteWithoutDates_KeepsOnlyTheFilters()
    {
        var route = Model(EventPeriod.Today).RouteWithoutDates();

        Assert.False(route.ContainsKey("period"));
        Assert.False(route.ContainsKey("fromDate"));
        Assert.Equal("Music", route["category"]);
    }
}
