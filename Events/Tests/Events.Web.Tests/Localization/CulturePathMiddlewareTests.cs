using Events.Web.Localization;
using Microsoft.AspNetCore.Http;

namespace Events.Web.Tests.Localization;

public class CulturePathMiddlewareTests
{
    private sealed record Seen(string PathBase, string Path, bool IsEnglish);

    private static async Task<(DefaultHttpContext Context, Seen? Seen)> InvokeAsync(
        string path, string query = "", string method = "GET")
    {
        Seen? seen = null;
        var middleware = new CulturePathMiddleware(ctx =>
        {
            seen = new Seen(ctx.Request.PathBase.Value ?? "", ctx.Request.Path.Value ?? "", SiteCultures.IsEnglishRequest(ctx));
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.QueryString = new QueryString(query);

        await middleware.InvokeAsync(context);
        return (context, seen);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Events")]
    [InlineData("/english")]
    [InlineData("/venues/ndk")]
    public async Task BulgarianPath_PassesThroughUnchanged(string path)
    {
        var (_, seen) = await InvokeAsync(path);

        Assert.NotNull(seen);
        Assert.Equal("", seen!.PathBase);
        Assert.Equal(path, seen.Path);
        Assert.False(seen.IsEnglish);
    }

    [Theory]
    [InlineData("/en/", "/")]
    [InlineData("/en", "/")]
    [InlineData("/en/Events/Details/1593", "/Events/Details/1593")]
    [InlineData("/en/venues/ndk", "/venues/ndk")]
    [InlineData("/en/Identity/Account/Login", "/Identity/Account/Login")]
    public async Task EnglishPath_MovesPrefixIntoPathBase(string path, string expectedPath)
    {
        var (_, seen) = await InvokeAsync(path, "?category=Music");

        Assert.NotNull(seen);
        Assert.Equal("/en", seen!.PathBase);
        Assert.Equal(expectedPath, seen.Path);
        Assert.True(seen.IsEnglish);
    }

    [Fact]
    public async Task EnglishPath_RestoresPathAfterwards()
    {
        var (context, _) = await InvokeAsync("/en/Events");

        Assert.Equal("", context.Request.PathBase.Value ?? "");
        Assert.Equal("/en/Events", context.Request.Path.Value);
    }

    [Theory]
    [InlineData("/EN", "?x=1", "/en/?x=1")]
    [InlineData("/EN/Events", "?category=Music", "/en/Events?category=Music")]
    [InlineData("/En/", "", "/en/")]
    [InlineData("/en/Admin/Events", "?page=2", "/Admin/Events?page=2")]
    [InlineData("/en/admin", "", "/admin")]
    [InlineData("/en/api/tags/all", "", "/api/tags/all")]
    [InlineData("/en/sitemap.xml", "", "/sitemap.xml")]
    [InlineData("/en/robots.txt", "", "/robots.txt")]
    public async Task NonCanonicalEnglishPath_RedirectsPermanently(string path, string query, string expectedLocation)
    {
        var (context, seen) = await InvokeAsync(path, query);

        Assert.Null(seen);
        Assert.Equal(StatusCodes.Status301MovedPermanently, context.Response.StatusCode);
        Assert.Equal(expectedLocation, context.Response.Headers.Location.ToString());
    }

    [Fact]
    public async Task NonCanonicalEnglishPath_Post_KeepsMethod()
    {
        var (context, _) = await InvokeAsync("/en/api/favorites/toggle/5", method: "POST");

        Assert.Equal(StatusCodes.Status308PermanentRedirect, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("", "/Events", "?category=Music", "en", "/en/Events?category=Music")]
    [InlineData("/en", "/Events", "?category=Music", "bg", "/Events?category=Music")]
    [InlineData("", "/", "", "en", "/en/")]
    [InlineData("/en", "/", "", "bg", "/")]
    [InlineData("/en", "/venues/ndk", "", "en", "/en/venues/ndk")]
    public void BuildPathForCulture_SamePageAndQueryInTheOtherLanguage(
        string pathBase, string path, string query, string culture, string expected)
    {
        var context = new DefaultHttpContext();
        context.Request.PathBase = pathBase;
        context.Request.Path = path;
        context.Request.QueryString = new QueryString(query);

        Assert.Equal(expected, SiteCultures.BuildPathForCulture(context.Request, culture));
    }

    [Theory]
    [InlineData("/en", "en")]
    [InlineData("", "bg")]
    public async Task UrlPathRequestCultureProvider_UsesOnlyPathBase(string pathBase, string expectedCulture)
    {
        var context = new DefaultHttpContext();
        context.Request.PathBase = pathBase;
        context.Request.Headers.AcceptLanguage = "en-US,en;q=0.9";
        context.Request.Headers.Cookie = ".AspNetCore.Culture=c=en|uic=en";

        var result = await new UrlPathRequestCultureProvider().DetermineProviderCultureResult(context);

        Assert.NotNull(result);
        Assert.Equal(expectedCulture, result!.UICultures.Single().Value);
    }
}
