using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Web.Models;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;

    public EventsController(ILogger<EventsController> logger, IEventService eventService)
    {
        _logger = logger;
        _eventService = eventService;
    }

    // GET: Admin/Events
    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? search = null, string? category = null)
    {
        try
        {
            IEnumerable<Event> events;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchResults = await _eventService.SearchEventsAsync(search);
                events = searchResults;
                totalCount = events.Count();
            }
            else
            {
                var result = await _eventService.GetPagedEventsAsync(page, pageSize, null, category, null, null);
                events = result.Events;
                totalCount = result.TotalCount;
            }

            var eventViewModels = AdminEventViewModel.FromEntities(events);
            var paginatedEvents = new PaginatedList<AdminEventViewModel>(eventViewModels, totalCount, page, pageSize);

            ViewBag.SearchTerm = search;
            ViewBag.Category = category;

            return View(paginatedEvents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin events list");
            return View(new PaginatedList<AdminEventViewModel>(new List<AdminEventViewModel>(), 0, 1, pageSize));
        }
    }

    // GET: Admin/Events/Featured
    public async Task<IActionResult> Featured(int page = 1, int pageSize = 20, string? search = null)
    {
        try
        {
            var allEvents = await _eventService.GetAllEventsAsync();

            // Get featured events
            var featuredEvents = allEvents.Where(e => e.IsFeatured).OrderByDescending(e => e.UpdatedAt).ToList();

            // Get available events for featuring
            IEnumerable<Event> availableEvents;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchResults = await _eventService.SearchEventsAsync(search);
                availableEvents = searchResults.Where(e => !e.IsFeatured && e.Status == EventStatus.Published);
            }
            else
            {
                availableEvents = allEvents.Where(e => !e.IsFeatured && e.Status == EventStatus.Published).OrderBy(e => e.Date);
            }

            var totalAvailable = availableEvents.Count();
            var pagedAvailable = availableEvents.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new FeaturedEventManagementViewModel
            {
                FeaturedEvents = AdminEventViewModel.FromEntities(featuredEvents),
                AvailableEvents = new PaginatedList<AdminEventViewModel>(
                    AdminEventViewModel.FromEntities(pagedAvailable),
                    totalAvailable,
                    page,
                    pageSize),
                SearchTerm = search
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured events management");
            return View(new FeaturedEventManagementViewModel());
        }
    }

    // POST: Admin/Events/SetFeatured
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetFeatured(int id, bool isFeatured)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            eventEntity.IsFeatured = isFeatured;
            await _eventService.UpdateEventAsync(eventEntity);

            _logger.LogInformation("Event {EventId} featured status changed to {IsFeatured}", id, isFeatured);

            TempData["SuccessMessage"] = isFeatured 
                ? $"Event '{eventEntity.Name}' has been added to featured events." 
                : $"Event '{eventEntity.Name}' has been removed from featured events.";

            return RedirectToAction(nameof(Featured));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting featured status for event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the event.";
            return RedirectToAction(nameof(Featured));
        }
    }

    // GET: Admin/Events/Uncategorized
    public async Task<IActionResult> Uncategorized(int page = 1, int pageSize = 20, string? search = null)
    {
        try
        {
            var allEvents = await _eventService.GetAllEventsAsync();
            
            // Filter uncategorized events (CategoryId = 11 is Undefined)
            var uncategorizedEvents = allEvents.Where(e => e.CategoryId == 11);

            if (!string.IsNullOrWhiteSpace(search))
            {
                uncategorizedEvents = uncategorizedEvents.Where(e =>
                    e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (e.Description != null && e.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var totalCount = uncategorizedEvents.Count();
            var pagedEvents = uncategorizedEvents
                .OrderBy(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UncategorizedEventsViewModel
            {
                Events = new PaginatedList<AdminEventViewModel>(
                    AdminEventViewModel.FromEntities(pagedEvents),
                    totalCount,
                    page,
                    pageSize),
                SearchTerm = search
            };

            // TODO: Add available categories and subcategories from repository
            // This will be populated in next step when we add category service methods

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading uncategorized events");
            return View(new UncategorizedEventsViewModel());
        }
    }

    // POST: Admin/Events/UpdateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(int id, int categoryId, int? subCategoryId)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            eventEntity.CategoryId = categoryId;
            eventEntity.SubCategoryId = subCategoryId;
            await _eventService.UpdateEventAsync(eventEntity);

            _logger.LogInformation("Event {EventId} category updated to {CategoryId}", id, categoryId);

            TempData["SuccessMessage"] = $"Event '{eventEntity.Name}' has been categorized successfully.";

            return RedirectToAction(nameof(Uncategorized));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category for event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the event category.";
            return RedirectToAction(nameof(Uncategorized));
        }
    }

    // GET: Admin/Events/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            var viewModel = new EditEventViewModel
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Date = eventEntity.Date,
                StartTime = eventEntity.StartTime,
                City = eventEntity.City,
                Location = eventEntity.Location,
                Description = eventEntity.Description,
                ImageUrl = eventEntity.ImageUrl,
                TicketUrl = eventEntity.TicketUrl,
                IsFree = eventEntity.IsFree,
                Price = eventEntity.Price,
                IsFeatured = eventEntity.IsFeatured,
                CategoryId = eventEntity.CategoryId,
                SubCategoryId = eventEntity.SubCategoryId,
                Status = eventEntity.Status,
                SourceUrl = eventEntity.SourceUrl
            };

            // TODO: Populate available categories and subcategories
            // This will be added in next step

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit page for event {EventId}", id);
            return NotFound();
        }
    }

    // POST: Admin/Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditEventViewModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // TODO: Repopulate available categories and subcategories
                return View(model);
            }

            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            // Update event properties
            eventEntity.Name = model.Name;
            eventEntity.Date = model.Date;
            eventEntity.StartTime = model.StartTime;
            eventEntity.City = model.City;
            eventEntity.Location = model.Location;
            eventEntity.Description = model.Description;
            eventEntity.ImageUrl = model.ImageUrl;
            eventEntity.TicketUrl = model.TicketUrl;
            eventEntity.IsFree = model.IsFree;
            eventEntity.Price = model.Price;
            eventEntity.IsFeatured = model.IsFeatured;
            eventEntity.CategoryId = model.CategoryId;
            eventEntity.SubCategoryId = model.SubCategoryId;
            eventEntity.Status = model.Status;

            await _eventService.UpdateEventAsync(eventEntity);

            _logger.LogInformation("Event {EventId} updated successfully", id);

            TempData["SuccessMessage"] = $"Event '{eventEntity.Name}' has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the event.";
            return View(model);
        }
    }

    // POST: Admin/Events/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            await _eventService.DeleteEventAsync(id);

            _logger.LogInformation("Event {EventId} deleted successfully", id);

            TempData["SuccessMessage"] = $"Event '{eventEntity.Name}' has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the event.";
            return RedirectToAction(nameof(Index));
        }
    }
}
