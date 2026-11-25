namespace Events.Web.Infrastructure;

// Central place to resolve the site's absolute base URL, so controllers don't each
// build "$"{Request.Scheme}://{Request.Host}"" ad hoc (see VenuesController for the
// original inline version this replaces).
public interface ISiteUrlProvider
{
    // Absolute base URL for the current request, e.g. "https://go-sofia.com" (no trailing slash).
    string BaseUrl { get; }

    // Combines BaseUrl with a relative path, e.g. "/Events/Details/5" -> "https://go-sofia.com/Events/Details/5".
    // Language-neutral: for files such as images and the logo, which are the same for both languages.
    string BuildAbsoluteUrl(string relativePath);

    // Absolute URL of a page in the given language (GS-REQ-010, 10.3): "/Events/Details/5" ->
    // "https://www.go-sofia.com/Events/Details/5" (bg) or "https://www.go-sofia.com/en/Events/Details/5" (en).
    // pathAndQuery is language-neutral (without "/en").
    string BuildLanguageUrl(string culture, string pathAndQuery);

    // Absolute URL of a page in the language of the current request (canonical, og:url, JSON-LD, breadcrumbs).
    string BuildPageUrl(string pathAndQuery);
}
