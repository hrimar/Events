using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Web.Extensions;

namespace Events.Web.Tests.Extensions;

public class HomeQuickFilterExtensionsTests
{
    private static HomeQuickFilter Filter(HomeQuickFilterType type, params string[] values) =>
        new() { Type = type, Values = values.ToList() };

    [Fact]
    public void ToRouteValues_Categories_JoinsCategoriesForNext7Days()
    {
        var route = Filter(HomeQuickFilterType.Categories, "Business", "Conferences").ToRouteValues();

        Assert.Equal("next7", route["period"]);
        Assert.Equal("Business,Conferences", route["category"]);
        Assert.False(route.ContainsKey("tags"));
    }

    [Fact]
    public void ToRouteValues_Tags_JoinsTags()
    {
        var route = Filter(HomeQuickFilterType.Tags, "For kids", "Family-friendly").ToRouteValues();

        Assert.Equal("For kids,Family-friendly", route["tags"]);
    }

    [Fact]
    public void ToRouteValues_Free_SetsFreeOnly()
    {
        var route = Filter(HomeQuickFilterType.Free).ToRouteValues();

        Assert.Equal("true", route["free"]);
        Assert.False(route.ContainsKey("category"));
    }

    [Fact]
    public void ApplyTo_UsesTheSameFilterAsTheLink()
    {
        var query = new PublicEventQuery();

        Assert.Equal(new[] { "Business", "Conferences" },
            Filter(HomeQuickFilterType.Categories, "Business", "Conferences").ApplyTo(query).CategoryNames);
        Assert.Equal(new[] { "For kids" }, Filter(HomeQuickFilterType.Tags, "For kids").ApplyTo(query).TagNames);
        Assert.True(Filter(HomeQuickFilterType.Free).ApplyTo(query).IsFree);
    }
}
