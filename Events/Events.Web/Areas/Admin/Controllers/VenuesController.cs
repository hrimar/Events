using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class VenuesController : Controller
{
    private readonly IVenueService _venueService;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<VenuesController> _logger;

    public VenuesController(
        IVenueService venueService,
        IEventRepository eventRepository,
        ILogger<VenuesController> logger)
    {
        _venueService = venueService;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var venues = await _venueService.GetAllWithStatsAsync();

            var viewModel = venues.Select(v => new AdminVenueListItemViewModel
            {
                Id = v.Id,
                Name = v.Name,
                ShortName = v.ShortName,
                Slug = v.Slug,
                City = v.City,
                AliasCount = v.AliasCount,
                UpcomingEventsCount = v.UpcomingEventsCount,
                TotalEventsCount = v.TotalEventsCount
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading venues list");
            TempData["ErrorMessage"] = "Unable to load venues.";
            return View(new List<AdminVenueListItemViewModel>());
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new AdminVenueFormViewModel
            {
                Slug = await _venueService.GenerateUniqueSlugAsync(string.Empty)
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Create venue form");
            TempData["ErrorMessage"] = "Unable to load create form.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminVenueFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // If admin left slug empty, generate it from the name
            if (string.IsNullOrWhiteSpace(model.Slug))
                model.Slug = await _venueService.GenerateUniqueSlugAsync(model.Name);

            var venue = new CanonicalVenue
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Slug = model.Slug,
                Address = model.Address,
                City = model.City,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Description = model.Description,
                PhotoUrl = model.PhotoUrl,
                WebsiteUrl = model.WebsiteUrl,
                Capacity = model.Capacity
            };

            await _venueService.CreateAsync(venue);

            TempData["SuccessMessage"] = $"Venue '{venue.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating venue '{VenueName}'", model.Name);
            TempData["ErrorMessage"] = "Unable to create venue.";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
                return NotFound();

            var viewModel = new AdminVenueFormViewModel
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
                ExistingAliases = venue.Aliases.Select(a => new AdminVenueAliasViewModel
                {
                    Id = a.Id,
                    AliasString = a.AliasString,
                    NormalizedString = a.NormalizedString
                }).ToArray()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for venue {VenueId}", id);
            TempData["ErrorMessage"] = "Unable to load venue.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminVenueFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
                return NotFound();

            venue.Name = model.Name;
            venue.ShortName = model.ShortName;
            venue.Slug = model.Slug;
            venue.Address = model.Address;
            venue.City = model.City;
            venue.Latitude = model.Latitude;
            venue.Longitude = model.Longitude;
            venue.Description = model.Description;
            venue.PhotoUrl = model.PhotoUrl;
            venue.WebsiteUrl = model.WebsiteUrl;
            venue.Capacity = model.Capacity;
            venue.UpdatedAt = DateTime.UtcNow;

            await _venueService.UpdateAsync(venue);

            TempData["SuccessMessage"] = $"Venue '{venue.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating venue {VenueId}", id);
            TempData["ErrorMessage"] = "Unable to update venue.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _venueService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Venue deleted successfully.";
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Venue {VenueId} not found for deletion", id);
            TempData["ErrorMessage"] = "Venue not found.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting venue {VenueId}", id);
            TempData["ErrorMessage"] = "Unable to delete venue.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAlias(int venueId, string aliasString)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(aliasString))
            {
                TempData["ErrorMessage"] = "Alias string cannot be empty.";
                return RedirectToAction(nameof(Edit), new { id = venueId });
            }

            await _venueService.AddAliasAsync(venueId, aliasString);
            TempData["SuccessMessage"] = $"Alias '{aliasString}' added successfully.";
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid alias string '{AliasString}' for venue {VenueId}", aliasString, venueId);
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding alias '{AliasString}' to venue {VenueId}", aliasString, venueId);
            TempData["ErrorMessage"] = "Unable to add alias.";
        }

        return RedirectToAction(nameof(Edit), new { id = venueId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAlias(int aliasId, int venueId)
    {
        try
        {
            await _venueService.DeleteAliasAsync(aliasId);
            TempData["SuccessMessage"] = "Alias removed successfully.";
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Alias {AliasId} not found for deletion", aliasId);
            TempData["ErrorMessage"] = "Alias not found.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing alias {AliasId}", aliasId);
            TempData["ErrorMessage"] = "Unable to remove alias.";
        }

        return RedirectToAction(nameof(Edit), new { id = venueId });
    }

    public async Task<IActionResult> Unmapped()
    {
        try
        {
            var unmappedLocations = await _venueService.GetUnmappedLocationsAsync();
            var allVenues = await _venueService.GetAllAsync();

            var venueOptions = allVenues
                .OrderBy(v => v.Name)
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = string.IsNullOrEmpty(v.ShortName) ? v.Name : $"{v.Name} ({v.ShortName})"
                });

            var viewModel = new AdminUnmappedLocationsViewModel
            {
                Items = unmappedLocations.Select(u => new AdminUnmappedLocationItemViewModel
                {
                    Location = u.Location,
                    EventCount = u.EventCount
                }).ToArray(),
                VenueOptions = venueOptions,
                TotalUnmappedEvents = unmappedLocations.Sum(u => u.EventCount)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading unmapped locations");
            TempData["ErrorMessage"] = "Unable to load unmapped locations.";
            return View(new AdminUnmappedLocationsViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MapLocation(string location, int venueId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                TempData["ErrorMessage"] = "Location cannot be empty.";
                return RedirectToAction(nameof(Unmapped));
            }

            // Create alias so future crawls hit the fast exact-match path
            await _venueService.AddAliasAsync(venueId, location);

            // Bulk update all existing events with this location
            var updatedCount = await _eventRepository.UpdateCanonicalVenueByLocationAsync(location, venueId);

            TempData["SuccessMessage"] = $"Location '{location}' mapped successfully. {updatedCount} event(s) updated.";
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Cannot normalize location '{Location}'", location);
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping location '{Location}' to venue {VenueId}", location, venueId);
            TempData["ErrorMessage"] = "Unable to map location.";
        }

        return RedirectToAction(nameof(Unmapped));
    }
}
