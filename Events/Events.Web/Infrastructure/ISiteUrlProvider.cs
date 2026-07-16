namespace Events.Web.Infrastructure;

// Central place to resolve the site's absolute base URL, so controllers don't each
// build "$"{Request.Scheme}://{Request.Host}"" ad hoc (see VenuesController for the
// original inline version this replaces).
public interface ISiteUrlProvider
{
    // Absolute base URL for the current request, e.g. "https://go-sofia.com" (no trailing slash).
    string BaseUrl { get; }

    // Combines BaseUrl with a relative path, e.g. "/Events/Details/5" -> "https://go-sofia.com/Events/Details/5".
    string BuildAbsoluteUrl(string relativePath);
}
