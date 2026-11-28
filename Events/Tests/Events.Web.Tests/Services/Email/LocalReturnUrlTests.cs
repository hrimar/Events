using Events.Web.Services.Email;

namespace Events.Web.Tests.Services.Email;

public class LocalReturnUrlTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/add-event")]
    [InlineData("/en/add-event")]
    [InlineData("/en/add-event?x=1")]
    [InlineData("~/")]
    [InlineData("~/add-event")]
    public void OrNull_LocalUrl_IsKept(string url)
    {
        Assert.Equal(url, LocalReturnUrl.OrNull(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("https://evil.example/add-event")]
    [InlineData("//evil.example")]
    [InlineData("/\\evil.example")]
    [InlineData("~//evil.example")]
    [InlineData("add-event")]
    [InlineData("javascript:alert(1)")]
    public void OrNull_NotLocalUrl_IsNull(string? url)
    {
        Assert.Null(LocalReturnUrl.OrNull(url));
    }
}
