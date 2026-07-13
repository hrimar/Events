using AngleSharp.Dom;
using Ganss.Xss;

namespace Events.Services.Helpers;

// Sanitizes rich-text content from the Site Content admin editor down to a minimal safe set:
// bold, italic, links (forced target=_blank + rel=noopener noreferrer) and paragraphs.
// The sanitized output is safe to render with Html.Raw on public pages.
public static class RichTextSanitizer
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return Sanitizer.Sanitize(html);
    }

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.Clear();
        foreach (var tag in new[] { "p", "br", "b", "strong", "i", "em", "a" })
            sanitizer.AllowedTags.Add(tag);

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");

        sanitizer.AllowedSchemes.Clear();
        foreach (var scheme in new[] { "http", "https", "mailto" })
            sanitizer.AllowedSchemes.Add(scheme);

        // Force safe link behavior regardless of what the editor produced.
        sanitizer.PostProcessNode += (_, args) =>
        {
            if (args.Node is IElement element && element.TagName.Equals("A", StringComparison.OrdinalIgnoreCase))
            {
                element.SetAttribute("target", "_blank");
                element.SetAttribute("rel", "noopener noreferrer");
            }
        };

        return sanitizer;
    }
}
