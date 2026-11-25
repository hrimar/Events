using Events.Models;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Services.Time;
using Events.Web.Infrastructure;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Events.Web.Controllers;

public class VenuesController : Controller
{
    private readonly IVenueService _venueService;
    private readonly ILogger<VenuesController> _logger;
    private readonly ISiteUrlProvider _siteUrlProvider;
    private readonly ISeoMetaService _seoMetaService;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ISofiaClock _clock;

    public VenuesController(
        IVenueService venueService,
        ILogger<VenuesController> logger,
        ISiteUrlProvider siteUrlProvider,
        ISeoMetaService seoMetaService,
        IStringLocalizer<SharedResources> localizer,
        ISofiaClock clock)
    {
        _venueService = venueService;
        _logger = logger;
        _siteUrlProvider = siteUrlProvider;
        _seoMetaService = seoMetaService;
        _localizer = localizer;
        _clock = clock;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        try
        {
            var venues = await _venueService.GetAllWithStatsAsync();

            var viewModels = venues
                .Select(v => new VenueListItemViewModel
                {
                    Slug = v.Slug,
                    Name = v.Name,
                    NameEn = v.NameEn ?? string.Empty,
                    ShortName = v.ShortName,
                    PhotoUrl = v.PhotoUrl,
                    City = v.City,
                    UpcomingEventsCount = v.UpcomingEventsCount
                })
                .OrderByDescending(v => v.UpcomingEventsCount)
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                viewModels = viewModels
                    .Where(v => v.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || v.NameEn.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || (v.ShortName != null && v.ShortName.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            ViewBag.Search = search;
            ViewData["Title"] = _localizer["Venue_IndexTitle"].Value;

            // Default description until an admin configures PageSeoMeta for this page -
            // set before the lookup (not clobbered after), mirroring the Home page fix.
            var pageMetaDescription = _localizer["Venue_IndexSubtitle"].Value;

            var seo = await _seoMetaService.GetByKeyAsync(SeoPageKeys.VenuesIndex);
            if (seo != null)
            {
                var isEnglish = CultureHelper.IsEnglish();
                var seoTitle = seo.LocalizedTitle(isEnglish);
                if (!string.IsNullOrWhiteSpace(seoTitle))
                    ViewData["Title"] = seoTitle;

                var seoDescription = seo.LocalizedDescription(isEnglish);
                if (!string.IsNullOrWhiteSpace(seoDescription))
                    pageMetaDescription = seoDescription;
            }

            ViewData["MetaDescription"] = pageMetaDescription;
            ViewData["OgImageUrl"] = _siteUrlProvider.BuildAbsoluteUrl("/images/logo.jpeg");
            // Language-neutral path; _Layout builds the canonical, og:url and hreflang from it (GS-REQ-010, 10.3).
            ViewData["CanonicalPath"] = "/Venues";

            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading venues index");
            return View(new List<VenueListItemViewModel>());
        }
    }

    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var venue = await _venueService.GetBySlugAsync(slug);
        if (venue == null)
            return NotFound();

        var upcomingEvents = (await _venueService.GetUpcomingEventsByVenueAsync(venue.Id)).ToList();
        var eventViewModels = EventViewModel.FromEntities(upcomingEvents, _clock.Today).AsReadOnly();

        var viewModel = new VenueDetailsViewModel
        {
            Id = venue.Id,
            Name = venue.Name,
            NameEn = venue.NameEn,
            ShortName = venue.ShortName,
            Slug = venue.Slug,
            Address = venue.Address,
            City = venue.City,
            Latitude = venue.Latitude,
            Longitude = venue.Longitude,
            Description = venue.Description,
            PhotoUrl = venue.PhotoUrl,
            WebsiteUrl = venue.WebsiteUrl,
            Capacity = venue.Capacity,
            UpcomingEvents = eventViewModels,
            JsonLd = BuildJsonLd(venue, upcomingEvents)
        };

        return View(viewModel);
    }

    // JSON-LD in the language of the page, with its own-language URLs (GS-REQ-010, 10.3).
    private string BuildJsonLd(CanonicalVenue venue, IReadOnlyList<Event> events)
    {
        var language = SiteCultures.ForRequest(HttpContext);
        var ownPageUrl = _siteUrlProvider.BuildPageUrl($"/venues/{venue.Slug}");
        var place = PlaceJsonLdBuilder.BuildPlace(venue, ownPageUrl, includeContext: false, language == SiteCultures.English);

        if (events.Any())
            place["event"] = events
                .Select(ev => EventJsonLdBuilder.BuildEvent(ev, _siteUrlProvider.BaseUrl, includeContext: false, language))
                .ToList();

        var breadcrumb = BreadcrumbJsonLdBuilder.BuildBreadcrumbList(BuildBreadcrumbItems(venue), includeContext: false);

        return SafeJsonLdBuilder.Serialize(SafeJsonLdBuilder.BuildGraph(place, breadcrumb));
    }

    // Mirrors the visible <nav aria-label="breadcrumb"> markup in Venues/Details.cshtml
    // (Home > VenueName - there is no intermediate "Venues" crumb in that view today).
    private List<(string Name, string? Url)> BuildBreadcrumbItems(CanonicalVenue venue)
    {
        var displayName = venue.LocalizedName(CultureHelper.IsEnglish());

        return new List<(string Name, string? Url)>
        {
            (_localizer["Venue_Home"].Value, _siteUrlProvider.BuildPageUrl("/")),
            (displayName, null)
        };
    }
}
