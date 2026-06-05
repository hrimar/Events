using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Events.Web.Controllers;

public class VenuesController : Controller
{
    private readonly IVenueService _venueService;
    private readonly ILogger<VenuesController> _logger;

    public VenuesController(IVenueService venueService, ILogger<VenuesController> logger)
    {
        _venueService = venueService;
        _logger = logger;
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
        var place = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Place",
            ["name"] = venue.Name
        };

        if (!string.IsNullOrEmpty(venue.Description))
            place["description"] = venue.Description;

        if (!string.IsNullOrEmpty(venue.WebsiteUrl))
            place["url"] = venue.WebsiteUrl;

        if (!string.IsNullOrEmpty(venue.Address))
            place["address"] = new Dictionary<string, string>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = venue.Address,
                ["addressLocality"] = venue.City
            };

        if (venue.Latitude.HasValue && venue.Longitude.HasValue)
            place["geo"] = new Dictionary<string, object>
            {
                ["@type"] = "GeoCoordinates",
                ["latitude"] = venue.Latitude.Value,
                ["longitude"] = venue.Longitude.Value
            };

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

        return JsonSerializer.Serialize(place, new JsonSerializerOptions { WriteIndented = false });
    }
}
