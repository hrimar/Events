using Events.Services.Media;

namespace Events.Services.Tests.Media;

public class EventPageImageExtractorTests
{
    private static readonly Uri Page = new("https://tickets.example.bg/events/jazz-night?id=5");

    [Fact]
    public void Extract_OpenGraphImage_ReturnsDecodedAddress()
    {
        const string html = """
            <html><head>
            <meta property="og:image" content="https://scontent.fbcdn.net/v/t39/photo.jpg?stp=dst-jpg&amp;oh=00_AYB&amp;oe=6A1B2C3D">
            </head><body></body></html>
            """;

        var image = EventPageImageExtractor.Extract(html, Page);

        Assert.Equal("https://scontent.fbcdn.net/v/t39/photo.jpg?stp=dst-jpg&oh=00_AYB&oe=6A1B2C3D", image);
    }

    [Fact]
    public void Extract_RelativeOpenGraphImage_ResolvedAgainstPage()
    {
        const string html = """<meta property="og:image" content="/img/jazz.jpg">""";

        Assert.Equal("https://tickets.example.bg/img/jazz.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_OpenGraphWithNameAttributeAndOtherCase_IsFound()
    {
        const string html = """<meta name="OG:IMAGE" content="https://cdn.example.bg/a.png">""";

        Assert.Equal("https://cdn.example.bg/a.png", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_EmptyOpenGraph_FallsBackToTwitterImage()
    {
        const string html = """
            <meta property="og:image" content="  ">
            <meta name="twitter:image" content="https://cdn.example.bg/twitter.jpg">
            """;

        Assert.Equal("https://cdn.example.bg/twitter.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_OpenGraphHasPriorityOverTwitterAndJsonLd()
    {
        const string html = """
            <script type="application/ld+json">{"@type":"Event","image":"https://cdn.example.bg/ld.jpg"}</script>
            <meta name="twitter:image" content="https://cdn.example.bg/twitter.jpg">
            <meta property="og:image" content="https://cdn.example.bg/og.jpg">
            """;

        Assert.Equal("https://cdn.example.bg/og.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_JsonLdEventWithImageArray_ReturnsFirst()
    {
        const string html = """
            <script type="application/ld+json">
            { "@context": "https://schema.org", "@type": "Event", "name": "Jazz",
              "image": ["https://cdn.example.bg/1x1.jpg", "https://cdn.example.bg/16x9.jpg"] }
            </script>
            """;

        Assert.Equal("https://cdn.example.bg/1x1.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_JsonLdSubtypeWithImageObject_ReturnsUrl()
    {
        const string html = """
            <script type="application/ld+json">
            { "@type": "MusicEvent", "image": { "@type": "ImageObject", "url": "https://cdn.example.bg/music.webp" } }
            </script>
            """;

        Assert.Equal("https://cdn.example.bg/music.webp", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_JsonLdGraphWithFullTypeUri_FindsEvent()
    {
        const string html = """
            <script type="application/ld+json">
            { "@context": "https://schema.org", "@graph": [
                { "@type": "Organization", "logo": "https://cdn.example.bg/logo.png", "image": "https://cdn.example.bg/org.png" },
                { "@type": ["https://schema.org/TheaterEvent"], "image": "/media/play.jpg" } ] }
            </script>
            """;

        Assert.Equal("https://tickets.example.bg/media/play.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_JsonLdWithoutEvent_ReturnsNull()
    {
        const string html = """
            <script type="application/ld+json">{ "@type": "Organization", "image": "https://cdn.example.bg/org.png" }</script>
            """;

        Assert.Null(EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_BrokenJsonLdBeforeValidOne_UsesValidOne()
    {
        const string html = """
            <script type="application/ld+json">{ "@type": "Event", "image": </script>
            <script type="application/ld+json">[{ "@type": "Festival", "image": "https://cdn.example.bg/fest.jpg", }]</script>
            """;

        Assert.Equal("https://cdn.example.bg/fest.jpg", EventPageImageExtractor.Extract(html, Page));
    }

    [Fact]
    public void Extract_FacebookLoginPage_ReturnsNull()
    {
        // What Facebook typically returns for an event page without a session (GS-REQ-009, 9.2.2).
        const string html = """
            <!DOCTYPE html><html lang="bg"><head><title>Facebook</title>
            <meta name="description" content="Влезте във Facebook">
            <meta property="og:site_name" content="Facebook">
            </head><body><form id="login_form"></form></body></html>
            """;

        Assert.Null(EventPageImageExtractor.Extract(html, new Uri("https://www.facebook.com/events/123456789/")));
    }

    [Theory]
    [InlineData("data:image/png;base64,iVBORw0KGgo=")]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://cdn.example.bg/a.jpg")]
    public void Extract_NonHttpAddress_IsIgnored(string content)
    {
        var html = $"""<meta property="og:image" content="{content}">""";

        Assert.Null(EventPageImageExtractor.Extract(html, Page));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not html at all")]
    public void Extract_NoImage_ReturnsNull(string html)
    {
        Assert.Null(EventPageImageExtractor.Extract(html, Page));
    }
}
