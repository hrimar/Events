using Events.Web.Models;

namespace Events.Web.Tests.Models;

// srcset for images stored with variants (GS-REQ-009, 9.4).
public class EventImageSourcesTests
{
    private const string Id = "0123456789abcdef0123456789abcdef";
    private const string Expected = "/media/480/" + Id + ".webp 480w, /media/1200/" + Id + ".webp 1200w";

    [Theory]
    [InlineData("/media/1200/" + Id + ".webp")]
    [InlineData("/media/480/" + Id + ".webp")]
    public void BuildSrcSet_VariantAddress_ReturnsBothVariants(string imageUrl)
    {
        Assert.Equal(Expected, EventImageSources.BuildSrcSet(imageUrl));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("/images/default_event_image.jpeg")]
    [InlineData("/media/original/12_638000000000000000.jpg")]
    [InlineData("https://www.eventim.bg/obj/media/BG-eventim/teaser/222x222/2026/photo.jpg")]
    [InlineData("https://www.go-sofia.com/media/1200/" + Id + ".webp")]
    public void BuildSrcSet_OtherAddress_ReturnsNull(string? imageUrl)
    {
        Assert.Null(EventImageSources.BuildSrcSet(imageUrl));
    }

    [Fact]
    public void SizesFor_OnlyWithSrcSet()
    {
        Assert.Equal(EventImageSources.CardSizes, EventImageSources.SizesFor(Expected, EventImageSources.CardSizes));
        Assert.Null(EventImageSources.SizesFor(null, EventImageSources.CardSizes));
    }
}
