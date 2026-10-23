using System.Text.Encodings.Web;
using Events.Web.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Events.Web.Tests.TagHelpers;

public class IconTagHelperTests
{
    [Fact]
    public void Process_RendersDecorativeSvgThatUsesVersionedSpriteSymbol()
    {
        var output = Render(new IconTagHelper(new FakeFileVersionProvider(), HtmlEncoder.Default) { Name = "search" });

        Assert.Equal("svg", output.TagName);
        Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
        Assert.Equal("gs-icon", output.Attributes["class"].Value.ToString());
        Assert.Equal("true", output.Attributes["aria-hidden"].Value);
        Assert.Equal("false", output.Attributes["focusable"].Value);
        Assert.Equal("<use href=\"/icons/gs-icons.svg?v=abc#search\"></use>", output.Content.GetContent());
        Assert.False(output.Attributes.ContainsName("width"));
        Assert.False(output.Attributes.ContainsName("height"));
    }

    [Fact]
    public void Process_WithSize_SetsWidthAndHeight()
    {
        var output = Render(new IconTagHelper(new FakeFileVersionProvider(), HtmlEncoder.Default) { Name = "moon", Size = 20 });

        Assert.Equal(20, output.Attributes["width"].Value);
        Assert.Equal(20, output.Attributes["height"].Value);
    }

    [Fact]
    public void Process_KeepsExistingCssClasses()
    {
        var output = Render(
            new IconTagHelper(new FakeFileVersionProvider(), HtmlEncoder.Default) { Name = "map-pin" },
            new TagHelperAttributeList { { "class", "me-1" } });

        var classes = output.Attributes["class"].Value.ToString()!.Split(' ');
        Assert.Contains("me-1", classes);
        Assert.Contains("gs-icon", classes);
    }

    private static TagHelperOutput Render(IconTagHelper tagHelper, TagHelperAttributeList? attributes = null)
    {
        tagHelper.ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() };

        var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
        var output = new TagHelperOutput(
            "gs-icon",
            attributes ?? new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        tagHelper.Process(context, output);
        return output;
    }

    private sealed class FakeFileVersionProvider : IFileVersionProvider
    {
        public string AddFileVersionToPath(PathString requestPathBase, string path) => $"{requestPathBase}{path}?v=abc";
    }
}
