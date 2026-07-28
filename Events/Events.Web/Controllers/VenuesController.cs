using Events.Models;
using Events.Models.Entities;
using Events.Services.Interfaces;
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

    public VenuesController(
        IVenueService venueService,
        ILogger<VenuesController> logger,
        ISiteUrlProvider siteUrlProvider,
        ISeoMetaService seoMetaService,
        IStringLocalizer<SharedResources> localizer)
    {
        _venueService = venueService;
        _logger = logger;
        _siteUrlProvider = siteUrlProvider;
        _seoMetaService = seoMetaService;
        _localizer = localizer;
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
            ViewData["OgUrl"] = _siteUrlProvider.BuildAbsoluteUrl("/Venues");

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
        var eventViewModels = EventViewModel.FromEntities(upcomingEvents).AsReadOnly();

        var baseUrl = _siteUrlProvider.BaseUrl;

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
            JsonLd = BuildJsonLd(venue, upcomingEvents, baseUrl)
        };

        return View(viewModel);
    }

    private string BuildJsonLd(CanonicalVenue venue, IReadOnlyList<Event> events, string baseUrl)
    {
        var ownPageUrl = $"{baseUrl}/venues/{venue.Slug}";
        var place = PlaceJsonLdBuilder.BuildPlace(venue, ownPageUrl, includeContext: false);

        if (events.Any())
            place["event"] = events
                .Select(ev => EventJsonLdBuilder.BuildEvent(ev, baseUrl, includeContext: false))
                .ToList();

        var breadcrumb = BreadcrumbJsonLdBuilder.BuildBreadcrumbList(BuildBreadcrumbItems(venue, baseUrl), includeContext: false);

        return SafeJsonLdBuilder.Serialize(SafeJsonLdBuilder.BuildGraph(place, breadcrumb));
    }

    // Mirrors the visible <nav aria-label="breadcrumb"> markup in Venues/Details.cshtml
    // (Home > VenueName - there is no intermediate "Venues" crumb in that view today).
    private List<(string Name, string? Url)> BuildBreadcrumbItems(CanonicalVenue venue, string baseUrl)
    {
        var displayName = CultureHelper.IsEnglish() && !string.IsNullOrEmpty(venue.NameEn) ? venue.NameEn : venue.Name;

        return new List<(string Name, string? Url)>
        {
            (_localizer["Venue_Home"].Value, $"{baseUrl}/"),
            (displayName, null)
        };
    }
}
