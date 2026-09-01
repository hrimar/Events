using Events.Models.Enums;
using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class TagViewModelTests
{
    [Theory]
    [InlineData(0, "bg-light text-dark")]
    [InlineData(4, "bg-light text-dark")]
    [InlineData(5, "bg-secondary")]
    [InlineData(9, "bg-secondary")]
    [InlineData(10, "bg-info")]
    [InlineData(19, "bg-info")]
    [InlineData(20, "bg-primary")]
    [InlineData(100, "bg-primary")]
    public void BadgeClass_ReflectsEventCountThresholds(int eventCount, string expected)
    {
        var tag = new TagViewModel { EventCount = eventCount };

        Assert.Equal(expected, tag.BadgeClass);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(9, "")]
    [InlineData(10, "fs-6")]
    [InlineData(19, "fs-6")]
    [InlineData(20, "fs-5")]
    [InlineData(49, "fs-5")]
    [InlineData(50, "fs-4")]
    public void Size_ReflectsEventCountThresholds(int eventCount, string expected)
    {
        var tag = new TagViewModel { EventCount = eventCount };

        Assert.Equal(expected, tag.Size);
    }

    [Fact]
    public void CategoryIcon_KnownCategory_ReturnsMappedIcon()
    {
        var tag = new TagViewModel { Category = EventCategory.Music };

        Assert.Equal("fas fa-music", tag.CategoryIcon);
    }

    [Fact]
    public void CategoryIcon_NoCategory_ReturnsDefaultTagIcon()
    {
        var tag = new TagViewModel { Category = null };

        Assert.Equal("fas fa-tag", tag.CategoryIcon);
    }

    [Fact]
    public void DisplayName_ReturnsName()
    {
        var tag = new TagViewModel { Name = "Jazz" };

        Assert.Equal("Jazz", tag.DisplayName);
    }
}
