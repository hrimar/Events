using Events.Web.Extensions;
using Events.Web.Models;

namespace Events.Web.Tests.Extensions;

public class TagExtensionsTests
{
    private static TagViewModel CreateTagViewModel() => new() { Name = "Jazz", EventCount = 7 };

    [Fact]
    public void ToSuggestionDto_PrefixesNameWithHashAndFormatsEventCountAsLocation()
    {
        var result = CreateTagViewModel().ToSuggestionDto();

        Assert.Equal("#Jazz", result.Name);
        Assert.Equal("7 events", result.Location);
        Assert.True(result.IsTag);
        Assert.Equal("Jazz", result.TagName);
    }
}
