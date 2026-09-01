using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Tests.Infrastructure.JsonLd;

public class BreadcrumbJsonLdBuilderTests
{
    [Fact]
    public void BuildBreadcrumbList_AssignsSequentialOneBasedPositions()
    {
        var items = new List<(string Name, string? Url)>
        {
            ("Home", "/"),
            ("Events", "/events"),
            ("Concert", null) // current page - no link, matching the visible breadcrumb markup
        };

        var result = BreadcrumbJsonLdBuilder.BuildBreadcrumbList(items);

        var listItems = (List<Dictionary<string, object?>>)result["itemListElement"]!;
        Assert.Equal(1, listItems[0]["position"]);
        Assert.Equal(2, listItems[1]["position"]);
        Assert.Equal(3, listItems[2]["position"]);
    }

    [Fact]
    public void BuildBreadcrumbList_NullUrl_OmitsItemFieldForThatEntry()
    {
        var items = new List<(string Name, string? Url)> { ("Concert", null) };

        var result = BreadcrumbJsonLdBuilder.BuildBreadcrumbList(items);

        var listItems = (List<Dictionary<string, object?>>)result["itemListElement"]!;
        Assert.False(listItems[0].ContainsKey("item"));
    }

    [Fact]
    public void BuildBreadcrumbList_IncludeContextFalse_OmitsContextKey()
    {
        var result = BreadcrumbJsonLdBuilder.BuildBreadcrumbList([("Home", "/")], includeContext: false);

        Assert.False(result.ContainsKey("@context"));
    }

    [Fact]
    public void BuildBreadcrumbList_IncludeContextTrue_IncludesSchemaOrgContext()
    {
        var result = BreadcrumbJsonLdBuilder.BuildBreadcrumbList([("Home", "/")], includeContext: true);

        Assert.Equal("https://schema.org", result["@context"]);
    }
}
