using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;

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

        var viewModel = new VenueDetailsViewModel
        {
            Id = venue.Id,
            Name = venue.Name,
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
            UpcomingEvents = EventViewModel.FromEntities(upcomingEvents).AsReadOnly()
        };

        return View(viewModel);
    }
}
