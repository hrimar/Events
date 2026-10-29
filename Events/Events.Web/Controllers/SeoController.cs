using System.Text;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.Sitemap;
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
    public async Task<IActionResult> Sitemap()
    {
        var baseUrl = _siteUrlProvider.BaseUrl;
        var entries = new List<SitemapEntry>
        {
            new($"{baseUrl}/"),
            new($"{baseUrl}/Home/AboutUs"),
            new($"{baseUrl}/Events"),
            new($"{baseUrl}/Venues")
        };

        // The period pages are indexed and canonical (GS-REQ-008, 8.3); one entry per period, without categories,
        // whose combinations are reached through the links on these pages.
        foreach (var period in new[] { EventPeriod.Today, EventPeriod.Tomorrow, EventPeriod.Weekend, EventPeriod.Next7 })
            entries.Add(new SitemapEntry($"{baseUrl}/Events?period={EventPeriodCalculator.ToQueryValue(period)}"));

        foreach (EventCategory category in Enum.GetValues<EventCategory>())
        {
            if (category == EventCategory.Undefined)
                continue;

            // Matches EventsController.Index's "category" query filter (the working,
            // already-used listing convention), not the separate Category redirect action.
            entries.Add(new SitemapEntry($"{baseUrl}/Events?category={category}"));
        }

        // Only published, not-yet-past events - a stale sitemap entry for a finished
        // event has no SEO value and Google discourages listing past events at all.
        var (upcomingEvents, _) = await _eventService.GetPagedEventsAsync(new PublicEventQuery
        {
            PageSize = int.MaxValue,
            Status = EventStatus.Published,
            FromDate = _clock.Today
        });

        entries.AddRange(upcomingEvents.Select(e => new SitemapEntry($"{baseUrl}/Events/Details/{e.Id}", e.UpdatedAt)));

        var venues = await _venueService.GetAllAsync();

        entries.AddRange(venues.Select(v => new SitemapEntry($"{baseUrl}/venues/{v.Slug}", v.UpdatedAt)));

        var xml = SitemapBuilder.Serialize(SitemapBuilder.Build(entries));

        return Content(xml, "application/xml", Encoding.UTF8);
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

            $"\nSitemap: {baseUrl}/sitemap.xml\n";

        return Content(content, "text/plain", Encoding.UTF8);
    }
}
