using Events.Web.Localization;
using Microsoft.AspNetCore.Http;

namespace Events.Web.Infrastructure;

public class SiteUrlProvider : ISiteUrlProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SiteUrlProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string BaseUrl
    {
        get
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                return string.Empty;

            // Always force https for canonical/JSON-LD/sitemap URLs, even though
            // UseHttpsRedirection already upgrades http requests before they reach here.
            return $"https://{request.Host}";
        }
    }

    public string BuildAbsoluteUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return BaseUrl;

        return $"{BaseUrl}{(relativePath.StartsWith('/') ? relativePath : $"/{relativePath}")}";
    }

    public string BuildLanguageUrl(string culture, string pathAndQuery)
    {
        var path = string.IsNullOrWhiteSpace(pathAndQuery) ? "/" : pathAndQuery;
        if (!path.StartsWith('/'))
            path = $"/{path}";

        return $"{BaseUrl}{SiteCultures.GetPathPrefix(culture)}{path}";
    }

    public string BuildPageUrl(string pathAndQuery)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var culture = httpContext == null ? SiteCultures.Bulgarian : SiteCultures.ForRequest(httpContext);

        return BuildLanguageUrl(culture, pathAndQuery);
    }
}
