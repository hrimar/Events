using System.Text.Json;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Events.Services.Media;

// Finds the image of an event page when the import row has no "Image URL" (GS-REQ-009, 9.2.1):
// og:image, then twitter:image, then "image" of a JSON-LD Event (or an Event subtype such as MusicEvent).
// Attribute values come HTML-decoded from the parser ("&amp;" -> "&"); relative addresses are resolved against
// the page. Only http(s) addresses are returned; SafeHttpFetcher decides whether they may be downloaded.
public static class EventPageImageExtractor
{
    private static readonly string[] OpenGraphKeys = { "og:image", "og:image:url", "og:image:secure_url" };
    private static readonly string[] TwitterKeys = { "twitter:image", "twitter:image:src" };

    private static readonly JsonDocumentOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        MaxDepth = 32
    };

    public static string? Extract(string html, Uri pageUri)
    {
        ArgumentNullException.ThrowIfNull(pageUri);
        if (string.IsNullOrWhiteSpace(html))
        {
            return null;
        }

        var document = new HtmlParser().ParseDocument(html);

        return FindMetaImage(document, OpenGraphKeys, pageUri)
            ?? FindMetaImage(document, TwitterKeys, pageUri)
            ?? FindJsonLdEventImage(document, pageUri);
    }

    // <meta property="og:image" content="..."> (some sites use name= instead of property=).
    private static string? FindMetaImage(IDocument document, string[] keys, Uri pageUri)
    {
        foreach (var key in keys)
        {
            foreach (var meta in document.QuerySelectorAll("meta"))
            {
                var name = meta.GetAttribute("property") ?? meta.GetAttribute("name");
                if (!string.Equals(name?.Trim(), key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var resolved = ResolveImageUrl(meta.GetAttribute("content"), pageUri);
                if (resolved != null)
                {
                    return resolved;
                }
            }
        }

        return null;
    }

    private static string? FindJsonLdEventImage(IDocument document, Uri pageUri)
    {
        foreach (var script in document.QuerySelectorAll("script[type]"))
        {
            if (!string.Equals(script.GetAttribute("type")?.Trim(), "application/ld+json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            JsonDocument json;
            try
            {
                json = JsonDocument.Parse(script.TextContent, JsonOptions);
            }
            catch (JsonException)
            {
                // Broken JSON-LD on one script must not hide a valid one further down.
                continue;
            }

            using (json)
            {
                var image = FindEventImage(json.RootElement, pageUri);
                if (image != null)
                {
                    return image;
                }
            }
        }

        return null;
    }

    // Looks at the root object, a root array and "@graph" (one level, as used by schema.org generators).
    private static string? FindEventImage(JsonElement element, Uri pageUri)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var image = FindEventImage(item, pageUri);
                if (image != null)
                {
                    return image;
                }
            }

            return null;
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (IsEventType(element) && element.TryGetProperty("image", out var imageElement))
        {
            var image = ReadImage(imageElement, pageUri);
            if (image != null)
            {
                return image;
            }
        }

        return element.TryGetProperty("@graph", out var graph) ? FindEventImage(graph, pageUri) : null;
    }

    // "Event", any "...Event" subtype (MusicEvent, TheaterEvent, ScreeningEvent...) or "Festival"; @type may be an array.
    private static bool IsEventType(JsonElement element)
    {
        if (!element.TryGetProperty("@type", out var type))
        {
            return false;
        }

        return type.ValueKind switch
        {
            JsonValueKind.String => IsEventTypeName(type.GetString()),
            JsonValueKind.Array => type.EnumerateArray().Any(t => t.ValueKind == JsonValueKind.String && IsEventTypeName(t.GetString())),
            _ => false
        };
    }

    private static bool IsEventTypeName(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return false;
        }

        // "https://schema.org/MusicEvent" or "schema:MusicEvent" -> "MusicEvent".
        var name = typeName.Trim();
        var separator = name.LastIndexOfAny(new[] { '/', ':', '#' });
        if (separator >= 0)
        {
            name = name[(separator + 1)..];
        }

        return name.EndsWith("Event", StringComparison.Ordinal) || name == "Festival";
    }

    // "image" can be a string, an ImageObject ({ "url" } or { "contentUrl" }) or an array of either.
    private static string? ReadImage(JsonElement image, Uri pageUri)
    {
        switch (image.ValueKind)
        {
            case JsonValueKind.String:
                return ResolveImageUrl(image.GetString(), pageUri);

            case JsonValueKind.Object:
                foreach (var property in new[] { "url", "contentUrl" })
                {
                    if (image.TryGetProperty(property, out var url) && url.ValueKind == JsonValueKind.String)
                    {
                        var resolved = ResolveImageUrl(url.GetString(), pageUri);
                        if (resolved != null)
                        {
                            return resolved;
                        }
                    }
                }

                return null;

            case JsonValueKind.Array:
                foreach (var item in image.EnumerateArray())
                {
                    var resolved = ReadImage(item, pageUri);
                    if (resolved != null)
                    {
                        return resolved;
                    }
                }

                return null;

            default:
                return null;
        }
    }

    private static string? ResolveImageUrl(string? value, Uri pageUri)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Uri.TryCreate(pageUri, value.Trim(), out var absolute))
        {
            return null;
        }

        return absolute.Scheme == Uri.UriSchemeHttps || absolute.Scheme == Uri.UriSchemeHttp ? absolute.AbsoluteUri : null;
    }
}
