using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Events.Web.TagHelpers;

// Renders an icon from the Lucide sprite (wwwroot/icons/gs-icons.svg):
// <gs-icon name="search" /> or <gs-icon name="search" size="20" class="me-1" />
// The icon is decorative (aria-hidden); an icon-only button needs its own aria-label.
[HtmlTargetElement("gs-icon", Attributes = "name")]
public class IconTagHelper : TagHelper
{
    private const string SpritePath = "/icons/gs-icons.svg";

    private readonly IFileVersionProvider _fileVersionProvider;
    private readonly HtmlEncoder _htmlEncoder;

    public IconTagHelper(IFileVersionProvider fileVersionProvider, HtmlEncoder htmlEncoder)
    {
        _fileVersionProvider = fileVersionProvider;
        _htmlEncoder = htmlEncoder;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    // Symbol id in the sprite, i.e. the Lucide icon name
    public string Name { get; set; } = string.Empty;

    // Width and height in pixels; without it the icon follows the font size (1em)
    public int? Size { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // The version query string busts the browser cache when the sprite changes
        var spriteUrl = _fileVersionProvider.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, SpritePath);

        output.TagName = "svg";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.AddClass("gs-icon", _htmlEncoder);
        output.Attributes.SetAttribute("aria-hidden", "true");
        output.Attributes.SetAttribute("focusable", "false");

        if (Size is int size)
        {
            output.Attributes.SetAttribute("width", size);
            output.Attributes.SetAttribute("height", size);
        }

        output.Content.SetHtmlContent($"<use href=\"{_htmlEncoder.Encode($"{spriteUrl}#{Name}")}\"></use>");
    }
}
