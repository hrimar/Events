using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Models;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Events.Web.Controllers;

public class VenuesController : Controller
{
    private readonly IVenueService _venueService;
    private readonly ILogger<VenuesController> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public VenuesController(IVenueService venueService, ILogger<VenuesController> logger, IStringLocalizer<SharedResources> localizer)
    {
        _venueService = venueService;
        _logger = logger;
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

        var upcomingEvents = await _venueService.GetUpcomingEventsByVenueAsync(venue.Id);
        var eventViewModels = EventViewModel.FromEntities(upcomingEvents).AsReadOnly();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

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
            JsonLd = BuildJsonLd(venue, eventViewModels, baseUrl)
        };

        return View(viewModel);
    }

    private static string BuildJsonLd(CanonicalVenue venue, IReadOnlyList<EventViewModel> events, string baseUrl)
    {
        var place = PlaceJsonLdBuilder.BuildPlace(venue);

        if (events.Any())
            place["event"] = events.Select(ev => new Dictionary<string, object?>
            {
                ["@type"] = "Event",
                ["name"] = ev.Name,
                ["startDate"] = ev.Date.ToString("yyyy-MM-dd"),
                ["location"] = new Dictionary<string, string>
                {
                    ["@type"] = "Place",
                    ["name"] = venue.Name
                },
                ["url"] = $"{baseUrl}/Events/Details/{ev.Id}"
            }).ToList();

        return SafeJsonLdBuilder.Serialize(place);
    }
}
