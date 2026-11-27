using Microsoft.AspNetCore.Http;

namespace Events.Web.Localization;

// Serves the English version under "/en/" (GS-REQ-010, 10.1). The prefix is moved from Path into PathBase, so routing
// sees the same paths for both languages and every generated link (Url.Action, asp-* tag helpers, Url.Content("~/"),
// RedirectToAction) keeps "/en" automatically (GS-REQ-010, 10.2.5).
public sealed class CulturePathMiddleware
{
    // Sections without a language prefix (GS-REQ-010, 10.1): the admin panel is English-only and the API is
    // language-neutral; sitemap.xml and robots.txt exist once and list both languages (GS-REQ-010, 10.3).
    // Identity is allowed under "/en/" on purpose, so English visitors get an English login page.
    private static readonly PathString[] UnprefixedSections =
    {
        new("/Admin"), new("/api"), new("/sitemap.xml"), new("/robots.txt")
    };

    private readonly RequestDelegate _next;

    public CulturePathMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        if (!request.Path.StartsWithSegments(SiteCultures.EnglishPathPrefix, StringComparison.OrdinalIgnoreCase, out var matched, out var remaining))
        {
            await _next(context);
            return;
        }

        // "/en" is served as the English home page without a redirect: link generation renders the home page link as
        // "/en" (PathBase + "/"), and a redirect on every internal home link would cost a hop. The canonical URL of
        // the English home page is "/en/" (GS-REQ-010, 10.1, 10.3).
        if (!remaining.HasValue)
        {
            remaining = "/";
        }

        var redirectPath = GetRedirectPath(matched, remaining);
        if (redirectPath != null)
        {
            // 301 for GET/HEAD; 308 keeps the method for anything else (e.g. a form POST).
            var preserveMethod = !HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method);
            context.Response.Redirect($"{request.PathBase}{redirectPath}{request.QueryString}", permanent: true, preserveMethod);
            return;
        }

        var originalPathBase = request.PathBase;
        var originalPath = request.Path;

        request.PathBase = originalPathBase.Add(SiteCultures.EnglishPathPrefix);
        request.Path = remaining;

        try
        {
            await _next(context);
        }
        finally
        {
            request.PathBase = originalPathBase;
            request.Path = originalPath;
        }
    }

    // Returns the path to redirect to, or null when the request is served as English.
    private static string? GetRedirectPath(PathString matched, PathString remaining)
    {
        foreach (var section in UnprefixedSections)
        {
            // Escaped, not Value: the decoded path may contain non-ASCII characters, which are not allowed in the
            // Location header (Kestrel would answer 500).
            if (remaining.StartsWithSegments(section, StringComparison.OrdinalIgnoreCase))
                return remaining.ToUriComponent();
        }

        // "/EN/..." -> "/en/...": one spelling of the prefix.
        if (matched.Value != SiteCultures.EnglishPathPrefix.Value)
            return $"{SiteCultures.EnglishPathPrefix}{remaining}";

        return null;
    }
}
