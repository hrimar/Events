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
}
