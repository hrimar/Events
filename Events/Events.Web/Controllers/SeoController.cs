using System.Text;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.Sitemap;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

public class SeoController : Controller
{
    private readonly IEventService _eventService;
    private readonly IVenueService _venueService;
    private readonly ISiteUrlProvider _siteUrlProvider;

    public SeoController(IEventService eventService, IVenueService venueService, ISiteUrlProvider siteUrlProvider)
    {
        _eventService = eventService;
        _venueService = venueService;
        _siteUrlProvider = siteUrlProvider;
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
        var (upcomingEvents, _) = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, fromDate: DateTime.Today);

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
            $"\nSitemap: {baseUrl}/sitemap.xml\n";

        return Content(content, "text/plain", Encoding.UTF8);
    }
}
