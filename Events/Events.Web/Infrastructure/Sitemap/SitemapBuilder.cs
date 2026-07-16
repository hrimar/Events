using System.Text;
using System.Xml.Linq;

namespace Events.Web.Infrastructure.Sitemap;

// One <url> entry in the sitemap: an absolute URL and an optional last-modified date.
public record SitemapEntry(string Url, DateTime? LastModified = null);

// Builds a sitemap.xml document per https://www.sitemaps.org/protocol.html.
// Has no DB dependency - callers gather entries from wherever they come from
// (static pages, published events, venues) and pass them in as plain data.
public static class SitemapBuilder
{
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static XDocument Build(IEnumerable<SitemapEntry> entries)
    {
        var urlset = new XElement(Ns + "urlset");

        foreach (var entry in entries)
        {
            var url = new XElement(Ns + "url", new XElement(Ns + "loc", entry.Url));

            if (entry.LastModified.HasValue)
                url.Add(new XElement(Ns + "lastmod", entry.LastModified.Value.ToString("yyyy-MM-dd")));

            urlset.Add(url);
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), urlset);
    }

    // XDocument.ToString() strips the XML declaration, so we serialize through a
    // UTF-8 StringWriter to keep the <?xml ... encoding="utf-8"?> prolog intact.
    public static string Serialize(XDocument document)
    {
        using var writer = new Utf8StringWriter();
        document.Save(writer);
        return writer.ToString();
    }

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
