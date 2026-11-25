using Events.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Events.Web.Tests.Infrastructure;

public class SiteUrlProviderTests
{
    private static SiteUrlProvider CreateProvider(string pathBase = "")
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("www.go-sofia.com");
        context.Request.PathBase = pathBase;

        return new SiteUrlProvider(new HttpContextAccessor { HttpContext = context });
    }

    [Theory]
    [InlineData("bg", "/Events?category=Music", "https://www.go-sofia.com/Events?category=Music")]
    [InlineData("en", "/Events?category=Music", "https://www.go-sofia.com/en/Events?category=Music")]
    [InlineData("bg", "/", "https://www.go-sofia.com/")]
    [InlineData("en", "/", "https://www.go-sofia.com/en/")]
    [InlineData("en", "", "https://www.go-sofia.com/en/")]
    [InlineData("en", "venues/ndk", "https://www.go-sofia.com/en/venues/ndk")]
    public void BuildLanguageUrl_PrefixesOnlyEnglish(string culture, string path, string expected)
    {
        Assert.Equal(expected, CreateProvider().BuildLanguageUrl(culture, path));
    }

    [Theory]
    [InlineData("", "https://www.go-sofia.com/Events/Details/5")]
    [InlineData("/en", "https://www.go-sofia.com/en/Events/Details/5")]
    public void BuildPageUrl_UsesTheLanguageOfTheRequest(string pathBase, string expected)
    {
        Assert.Equal(expected, CreateProvider(pathBase).BuildPageUrl("/Events/Details/5"));
    }

    [Fact]
    public void BuildAbsoluteUrl_StaysLanguageNeutral()
    {
        Assert.Equal("https://www.go-sofia.com/images/logo.jpeg", CreateProvider("/en").BuildAbsoluteUrl("/images/logo.jpeg"));
    }
}
