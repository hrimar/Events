using Microsoft.AspNetCore.Http;

namespace Events.Web.Localization;

// The two site languages and the URL prefix of the English version (GS-REQ-010, 10.1).
// Bulgarian stays on the unprefixed URLs; English lives under "/en/".
public static class SiteCultures
{
    public const string Bulgarian = "bg";
    public const string English = "en";

    public static readonly string[] Supported = { Bulgarian, English };

    // Path prefix of the English version, without a trailing slash.
    public static readonly PathString EnglishPathPrefix = new("/en");

    // The language comes only from the URL (GS-REQ-010, 10.2.1): CulturePathMiddleware moves "/en" into PathBase.
    public static bool IsEnglishRequest(HttpContext httpContext) =>
        httpContext.Request.PathBase.Value?.EndsWith(EnglishPathPrefix.Value!, StringComparison.OrdinalIgnoreCase) == true;

    // Language of the current request, from the URL only ("bg" or "en").
    public static string ForRequest(HttpContext httpContext) => IsEnglishRequest(httpContext) ? English : Bulgarian;

    // URL path prefix of a language: "" for Bulgarian, "/en" for English (GS-REQ-010, 10.1).
    public static string GetPathPrefix(string culture) => culture == English ? EnglishPathPrefix.Value! : string.Empty;

    // Root-relative URL of the current page in the given language, with the same query string (GS-REQ-010, 10.2.4):
    // "/Events?category=Music" <-> "/en/Events?category=Music". Request.Path never contains "/en" (it is in PathBase).
    public static string BuildPathForCulture(HttpRequest request, string culture)
    {
        var path = request.Path.HasValue ? request.Path.Value! : "/";

        return $"{GetPathPrefix(culture)}{path}{request.QueryString}";
    }
}
