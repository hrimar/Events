using Events.Models.Enums;
using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class EventListFiltersTests
{
    private static EventListFilters Filters(EventPeriod? period) => new()
    {
        Period = period,
        FromDate = new DateTime(2026, 10, 30),
        ToDate = new DateTime(2026, 10, 30),
        CurrentCategory = "Music",
        SelectedSubCategory = "Jazz",
        IsFreeOnly = true,
        SelectedTags = new List<string> { "Blues" },
        IsKidsFilter = true
    };

    [Fact]
    public void RouteWithPeriod_KeepsTheFilters_AndDropsExplicitDates()
    {
        var route = Filters(period: null).RouteWithPeriod(EventPeriod.Weekend);

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
        var route = Filters(period: null).FilterRoute();

        Assert.Equal("2026-10-30", route["fromDate"]);
        Assert.Equal("2026-10-30", route["toDate"]);
        Assert.False(route.ContainsKey("period"));
    }

    [Fact]
    public void FilterRoute_KeepsTheSearchTerm()
    {
        var filters = Filters(EventPeriod.Next7);
        filters.SearchTerm = "jazz";

        Assert.Equal("jazz", filters.FilterRoute()["search"]);
        Assert.False(filters.RouteWith("search", null).ContainsKey("search"));
    }

    [Fact]
    public void RouteWith_NewCategory_ClearsTheSubcategory_AndKeepsThePeriod()
    {
        var route = Filters(EventPeriod.Next7).RouteWith("category", "Theatre");

        Assert.Equal("Theatre", route["category"]);
        Assert.False(route.ContainsKey("subCategory"));
        Assert.Equal("next7", route["period"]);
    }

    [Fact]
    public void RouteWith_NullValue_RemovesOnlyThatFilter()
    {
        var route = Filters(EventPeriod.Next7).RouteWith("free", null);

        Assert.False(route.ContainsKey("free"));
        Assert.Equal("Jazz", route["subCategory"]);
        Assert.Equal("true", route["kids"]);
    }

    [Fact]
    public void RouteWithoutDates_KeepsOnlyTheFilters()
    {
        var route = Filters(EventPeriod.Today).RouteWithoutDates();

        Assert.False(route.ContainsKey("period"));
        Assert.False(route.ContainsKey("fromDate"));
        Assert.Equal("Music", route["category"]);
    }

    [Fact]
    public void RouteWithTagToggled_AddsAMissingTag_AndRemovesASelectedOne()
    {
        var filters = Filters(EventPeriod.Next7);

        Assert.Equal("Blues,Rock", filters.RouteWithTagToggled("Rock")["tags"]);
        Assert.False(filters.RouteWithTagToggled("blues").ContainsKey("tags"));
    }

    [Fact]
    public void RouteForDay_KeepsTheFilters_AndSetsOnlyTheDay()
    {
        var route = Filters(EventPeriod.Next7).RouteForDay(new DateTime(2026, 10, 31));

        Assert.Equal("2026-10-31", route["day"]);
        Assert.False(route.ContainsKey("period"));
        Assert.Equal("Jazz", route["subCategory"]);
        Assert.Equal("true", route["kids"]);
    }

    [Fact]
    public void HasExplicitDates_NeedsBothDates_AndNoPeriod()
    {
        Assert.True(Filters(period: null).HasExplicitDates);
        Assert.False(Filters(EventPeriod.Today).HasExplicitDates);
        Assert.False(new EventListFilters { FromDate = new DateTime(2026, 10, 30) }.HasExplicitDates);
    }
}
