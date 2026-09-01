using Events.Web.Infrastructure.Sitemap;

namespace Events.Web.Tests.Infrastructure.Sitemap;

public class SitemapBuilderTests
{
    [Fact]
    public void Build_EntryWithoutLastModified_OmitsLastmodElement()
    {
        var document = SitemapBuilder.Build([new SitemapEntry("https://go-sofia.com/")]);

        var url = document.Root!.Elements().Single();
        Assert.Null(url.Elements().FirstOrDefault(e => e.Name.LocalName == "lastmod"));
    }

    [Fact]
    public void Build_EntryWithLastModified_FormatsAsDateOnly()
    {
        var document = SitemapBuilder.Build([new SitemapEntry("https://go-sofia.com/", new DateTime(2026, 6, 15, 14, 30, 0))]);

        var url = document.Root!.Elements().Single();
        var lastmod = url.Elements().Single(e => e.Name.LocalName == "lastmod");
        Assert.Equal("2026-06-15", lastmod.Value);
    }

    [Fact]
    public void Build_MultipleEntries_PreservesOrderAndUrls()
    {
        var document = SitemapBuilder.Build(
        [
            new SitemapEntry("https://go-sofia.com/a"),
            new SitemapEntry("https://go-sofia.com/b")
        ]);

        var locs = document.Root!.Elements().Select(url => url.Elements().Single(e => e.Name.LocalName == "loc").Value);
        Assert.Equal(["https://go-sofia.com/a", "https://go-sofia.com/b"], locs);
    }

    [Fact]
    public void Serialize_ProducesXmlDeclarationWithUtf8Encoding()
    {
        var document = SitemapBuilder.Build([new SitemapEntry("https://go-sofia.com/")]);

        var result = SitemapBuilder.Serialize(document);

        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>", result);
    }
}
