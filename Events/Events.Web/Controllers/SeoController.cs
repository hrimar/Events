using System.Text;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.Sitemap;
using Events.Web.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

public class SeoController : Controller
{
    private readonly IEventService _eventService;
    private readonly IVenueService _venueService;
    private readonly ISiteUrlProvider _siteUrlProvider;
    private readonly ISofiaClock _clock;

    public SeoController(IEventService eventService, IVenueService venueService, ISiteUrlProvider siteUrlProvider, ISofiaClock clock)
    {
        _eventService = eventService;
        _venueService = venueService;
        _siteUrlProvider = siteUrlProvider;
        _clock = clock;
    }

    // GET /sitemap.xml
    // Both language versions of each page, each listing the other through xhtml:link alternates (GS-REQ-010, 10.3).
    public async Task<IActionResult> Sitemap()
    {
        var entries = new List<SitemapEntry>();

        AddPage(entries, "/");
        AddPage(entries, "/Home/AboutUs");
        AddPage(entries, "/Events");
        AddPage(entries, "/Venues");

        // The period pages are indexed and canonical (GS-REQ-008, 8.3); one entry per period, without categories,
        // whose combinations are reached through the links on these pages.
        foreach (var period in new[] { EventPeriod.Today, EventPeriod.Tomorrow, EventPeriod.Weekend, EventPeriod.Next7 })
            AddPage(entries, $"/Events?period={EventPeriodCalculator.ToQueryValue(period)}");

        foreach (EventCategory category in Enum.GetValues<EventCategory>())
        {
            if (category == EventCategory.Undefined)
                continue;

            // Matches EventsController.Index's "category" query filter (the working,
            // already-used listing convention), not the separate Category redirect action.
            AddPage(entries, $"/Events?category={category}");
        }

        // Only published, not-yet-past events - a stale sitemap entry for a finished
        // event has no SEO value and Google discourages listing past events at all.
        var (upcomingEvents, _) = await _eventService.GetPagedEventsAsync(new PublicEventQuery
        {
            PageSize = int.MaxValue,
            Status = EventStatus.Published,
            FromDate = _clock.Today
        });

        // The English page of an event without an English title is not indexed, so it is left out (GS-REQ-010, 10.4).
        foreach (var e in upcomingEvents)
            AddPage(entries, $"/Events/Details/{e.Id}", e.UpdatedAt, includeEnglish: e.HasEnglishTitle());

        var venues = await _venueService.GetAllAsync();

        foreach (var v in venues)
            AddPage(entries, $"/venues/{v.Slug}", v.UpdatedAt);

        var xml = SitemapBuilder.Serialize(SitemapBuilder.Build(entries));

        return Content(xml, "application/xml", Encoding.UTF8);
    }

    // Adds the Bulgarian URL of a page and, when the page has an English version, the English URL too. Both carry the
    // same alternates (bg, en, x-default = bg); a Bulgarian-only page has none.
    private void AddPage(List<SitemapEntry> entries, string path, DateTime? lastModified = null, bool includeEnglish = true)
    {
        var bulgarianUrl = _siteUrlProvider.BuildLanguageUrl(SiteCultures.Bulgarian, path);

        if (!includeEnglish)
        {
            entries.Add(new SitemapEntry(bulgarianUrl, lastModified));
            return;
        }

        var englishUrl = _siteUrlProvider.BuildLanguageUrl(SiteCultures.English, path);
        var alternates = new[]
        {
            new SitemapAlternate(SiteCultures.Bulgarian, bulgarianUrl),
            new SitemapAlternate(SiteCultures.English, englishUrl),
            new SitemapAlternate("x-default", bulgarianUrl)
        };

        entries.Add(new SitemapEntry(bulgarianUrl, lastModified, alternates));
        entries.Add(new SitemapEntry(englishUrl, lastModified, alternates));
    }

    // GET /robots.txt
    public IActionResult Robots()
    {
        var baseUrl = _siteUrlProvider.BaseUrl;
        var content =
            "User-agent: *\n" +
            "Allow: /\n" +
            "Disallow: /Admin/\n" +   // admin area - no SEO value, has its own auth anyway
            "Disallow: /Identity/\n" + // login/register/manage account pages
            "Disallow: /api/\n" +     // JSON endpoints, not crawlable pages

            // Only the parameters whose pages must not be indexed are blocked (GS-REQ-008, 8.3). Sort, pages, tags,
            // subcategory, "free only" and "period" stay crawlable: their pages canonicalize to "/Events",
            // "/Events?category=X" or the period page (see EventsController), and Google can only read a canonical on
            // a page it is allowed to crawl. Explicit dates and search results (events and venues) are endless,
            // short-lived combinations;
            // "Paid only" (free=false) has no page of its own in the new design. The HTML fragments and JSON loaded by
            // the pages' scripts are not blocked here, because Google must fetch them to render the pages; they
            // send "X-Robots-Tag: noindex" instead (NoIndexAttribute).
            "Disallow: /Events?*fromDate=\n" +
            "Disallow: /Events?*toDate=\n" +
            "Disallow: /Events?*search=\n" +
            "Disallow: /Events?*free=false\n" +
            "Disallow: /Venues?*search=\n" +

            // The same rules for the English version under "/en/" (GS-REQ-010, 10.3). "/en/Admin" and "/en/api"
            // redirect to the unprefixed URLs (CulturePathMiddleware); Identity also works under "/en/".
            "Disallow: /en/Identity/\n" +
            "Disallow: /en/Events?*fromDate=\n" +
            "Disallow: /en/Events?*toDate=\n" +
            "Disallow: /en/Events?*search=\n" +
            "Disallow: /en/Events?*free=false\n" +
            "Disallow: /en/Venues?*search=\n" +

            $"\nSitemap: {baseUrl}/sitemap.xml\n";

        return Content(content, "text/plain", Encoding.UTF8);
    }
}
