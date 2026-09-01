using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Tests.Infrastructure.JsonLd;

public class SafeJsonLdBuilderTests
{
    [Fact]
    public void AddIfNotNull_NullValue_IsNotAdded()
    {
        var result = new SafeJsonLdBuilder().AddIfNotNull("key", null).Build();

        Assert.False(result.ContainsKey("key"));
    }

    [Fact]
    public void AddIfNotNull_NonNullValue_IsAdded()
    {
        var result = new SafeJsonLdBuilder().AddIfNotNull("key", 42).Build();

        Assert.Equal(42, result["key"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddIfNotEmpty_BlankValue_IsNotAdded(string? value)
    {
        var result = new SafeJsonLdBuilder().AddIfNotEmpty("key", value).Build();

        Assert.False(result.ContainsKey("key"));
    }

    [Fact]
    public void AddIfNotEmpty_NonBlankValue_IsAdded()
    {
        var result = new SafeJsonLdBuilder().AddIfNotEmpty("key", "value").Build();

        Assert.Equal("value", result["key"]);
    }

    [Fact]
    public void Add_OverwritesPreviousValueForSameKey()
    {
        var result = new SafeJsonLdBuilder().Add("key", "first").Add("key", "second").Build();

        Assert.Equal("second", result["key"]);
    }

    [Fact]
    public void BuildGraph_WrapsNodesUnderContextAndGraph()
    {
        var node1 = new Dictionary<string, object?> { ["@type"] = "Event" };
        var node2 = new Dictionary<string, object?> { ["@type"] = "Place" };

        var result = SafeJsonLdBuilder.BuildGraph(node1, node2);

        Assert.Equal("https://schema.org", result["@context"]);
        Assert.Equal(new object[] { node1, node2 }, result["@graph"]);
    }

    [Fact]
    public void Serialize_ProducesCompactJsonWithoutIndentation()
    {
        var value = new Dictionary<string, object?> { ["key"] = "value" };

        var result = SafeJsonLdBuilder.Serialize(value);

        Assert.DoesNotContain("\n", result);
        Assert.Equal("{\"key\":\"value\"}", result);
    }
}
