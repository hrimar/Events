using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers
{
    public class LanguageController : Controller
    {
        private static readonly HashSet<string> SupportedCultures = new(StringComparer.OrdinalIgnoreCase)
        {
            "bg", "en"
        };

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (!SupportedCultures.Contains(culture))
            {
                culture = "bg";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}