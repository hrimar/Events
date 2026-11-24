using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Events.Web.Localization;

// The only request culture provider (GS-REQ-010, 10.2.1-2): "/en/..." is English, everything else Bulgarian.
// Neither a cookie nor the browser's Accept-Language changes the language, so search engines see both versions.
public sealed class UrlPathRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var culture = SiteCultures.IsEnglishRequest(httpContext) ? SiteCultures.English : SiteCultures.Bulgarian;

        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture));
    }
}
