using System;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.Queries;
using Events.Web.Models;
using Events.Web.Models.Admin;
using Events.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class EventsController : Controller
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventService _eventService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ITagService _tagService;
    private readonly IImageUploadService _imageUploadService;
    private readonly IEventFilterOptionsBuilder _eventFilterOptionsBuilder;

    public EventsController(
        ILogger<EventsController> logger,
        IEventService eventService,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ITagService tagService,
        IImageUploadService imageUploadService,
        IEventFilterOptionsBuilder eventFilterOptionsBuilder)
    {
        _logger = logger;
        _eventService = eventService;
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _tagService = tagService;
        _imageUploadService = imageUploadService;
        _eventFilterOptionsBuilder = eventFilterOptionsBuilder;
    }

    // GET: Admin/Events
    public async Task<IActionResult> Index(EventListCriteria criteria)
    {
        try
        {
            var filter = criteria.Normalize();
            var categories = (await _categoryRepository.GetAllAsync()).ToList();
            var (events, totalCount) = await _eventService.GetFilteredEventsAsync(filter);

            var viewModel = new AdminEventsIndexViewModel
            {
                Events = new PaginatedList<AdminEventViewModel>(
                    AdminEventViewModel.FromEntities(events),
                    totalCount,
                    filter.Page,
                    filter.PageSize),
                Filters = filter,
                Options = await _eventFilterOptionsBuilder.BuildAdminOptionsAsync(filter, categories),
                CategoriesJson = JsonConvert.SerializeObject(categories.Select(c => new { id = c.Id, name = c.Name }))
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin events list");
            var emptyFilter = new EventListCriteria { PageSize = criteria.PageSize }.Normalize();
            var categories = (await _categoryRepository.GetAllAsync()).ToList();
            return View(new AdminEventsIndexViewModel
            {
                Events = new PaginatedList<AdminEventViewModel>(new List<AdminEventViewModel>(), 0, 1, emptyFilter.PageSize),
                Filters = emptyFilter,
                Options = await _eventFilterOptionsBuilder.BuildAdminOptionsAsync(emptyFilter, categories),
                CategoriesJson = JsonConvert.SerializeObject(categories.Select(c => new { id = c.Id, name = c.Name }))
            });
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

            // Load available categories
            var categories = await _categoryRepository.GetAllAsync();
            var availableCategories = categories
                .Where(c => c.Id != 11) // Exclude Undefined category
                .Select(c => new CategoryOption
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .OrderBy(c => c.Name)
                .ToList();

            // Load all subcategories grouped by category
            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            var subCategoriesDict = allSubCategories
                .GroupBy(sc => sc.CategoryId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(sc => new SubCategoryOption
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        CategoryId = sc.CategoryId
                    }).OrderBy(sc => sc.Name).ToList()
                );

            var viewModel = new UncategorizedEventsViewModel
            {
                Events = new PaginatedList<AdminEventViewModel>(
                    AdminEventViewModel.FromEntities(pagedEvents),
                    totalCount,
                    page,
                    pageSize),
                SearchTerm = search,
                AvailableCategories = availableCategories,
                AvailableSubCategories = subCategoriesDict
            };

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

            // Store old category for logging
            var oldCategoryId = eventEntity.CategoryId;

            // Update category and subcategory
            eventEntity.CategoryId = categoryId;
            eventEntity.SubCategoryId = subCategoryId;

            // Automatically set status to Published when categorizing from Undefined
            // CategoryId = 11 is Undefined
            if (oldCategoryId == 11 && categoryId != 11)
            {
                eventEntity.Status = EventStatus.Published;
                _logger.LogInformation("Event {EventId} status automatically changed to Published after categorization", id);
            }

            await _eventService.UpdateEventAsync(eventEntity);

            _logger.LogInformation("Event {EventId} category updated from {OldCategory} to {CategoryId}", id, oldCategoryId, categoryId);

            TempData["SuccessMessage"] = $"Event '{eventEntity.Name}' has been categorized successfully and set to Published status.";

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
                SourceUrl = eventEntity.SourceUrl,
                // Load currently selected tags
                SelectedTagIds = eventEntity.EventTags?.Select(et => et.TagId).ToList() ?? new List<int>()
            };

            // Load available categories
            var categories = await _categoryRepository.GetAllAsync();
            viewModel.AvailableCategories = categories.Select(c => new CategoryOption
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c => c.Name).ToList();

            // Load all subcategories (will be filtered by JavaScript)
            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            viewModel.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId
            }).OrderBy(sc => sc.Name).ToList();

            // Load available tags
            var allTags = await _tagService.GetAllTagsAsync();
            viewModel.AvailableTags = allTags
                .Select(t => new TagOption
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .OrderBy(t => t.Name)
                .ToList();

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
                // Repopulate available categories and subcategories
                var categories = await _categoryRepository.GetAllAsync();
                model.AvailableCategories = categories.Select(c => new CategoryOption
                {
                    Id = c.Id,
                    Name = c.Name
                }).OrderBy(c => c.Name).ToList();

                var allSubCategories = await _subCategoryRepository.GetAllAsync();
                model.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    CategoryId = sc.CategoryId
                }).OrderBy(sc => sc.Name).ToList();

                var allTags = await _tagService.GetAllTagsAsync();
                model.AvailableTags = allTags
                    .Select(t => new TagOption { Id = t.Id, Name = t.Name })
                    .OrderBy(t => t.Name)
                    .ToList();

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
            eventEntity.ThumbnailUrl = model.ThumbnailUrl;
            eventEntity.TicketUrl = model.TicketUrl;
            eventEntity.IsFree = model.IsFree;
            eventEntity.Price = model.Price;
            eventEntity.IsFeatured = model.IsFeatured;
            eventEntity.CategoryId = model.CategoryId;
            eventEntity.SubCategoryId = model.SubCategoryId;
            eventEntity.Status = model.Status;

            await _eventService.UpdateEventAsync(eventEntity);

            // Update tags - remove old and assign new ones
            if (model.SelectedTagIds.Any())
            {
                // Get current tag IDs
                var currentTagIds = eventEntity.EventTags?.Select(et => et.TagId).ToList() ?? new List<int>();

                // Remove tags that are no longer selected
                var tagsToRemove = currentTagIds.Except(model.SelectedTagIds).ToList();
                if (tagsToRemove.Any())
                {
                    foreach (var tagId in tagsToRemove)
                    {
                        await _tagService.RemoveTagFromEventAsync(eventEntity.Id, tagId);
                    }
                }

                // Add new tags
                var tagsToAdd = model.SelectedTagIds.Except(currentTagIds).ToList();
                if (tagsToAdd.Any())
                {
                    await _tagService.BulkAddTagsToEventAsync(eventEntity.Id, tagsToAdd);
                }
            }
            else
            {
                // Remove all tags if none selected
                var currentTagIds = eventEntity.EventTags?.Select(et => et.TagId).ToList() ?? new List<int>();
                foreach (var tagId in currentTagIds)
                {
                    await _tagService.RemoveTagFromEventAsync(eventEntity.Id, tagId);
                }
            }

            _logger.LogInformation("Event {EventId} updated successfully with tags", id);

            TempData["SuccessMessage"] = $"Event '{eventEntity.Name}' has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the event.";

            // Repopulate dropdowns on error
            var categories = await _categoryRepository.GetAllAsync();
            model.AvailableCategories = categories.Select(c => new CategoryOption
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c => c.Name).ToList();

            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            model.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId
            }).OrderBy(sc => sc.Name).ToList();

            var allTags = await _tagService.GetAllTagsAsync();
            model.AvailableTags = allTags
                .Select(t => new TagOption { Id = t.Id, Name = t.Name })
                .OrderBy(t => t.Name)
                .ToList();

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

            var eventName = eventEntity.Name;

            // Delete associated images from Azure Blob Storage before deleting the event
            if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
            {
                var originalDeleted = await _imageUploadService.DeleteImageAsync(eventEntity.ImageUrl);
                if (originalDeleted)
                {
                    _logger.LogInformation("Deleted original image for event {EventId}: {ImageUrl}", id, eventEntity.ImageUrl);
                }
                else
                {
                    _logger.LogWarning("Failed to delete original image for event {EventId}: {ImageUrl}", id, eventEntity.ImageUrl);
                }
            }

            if (!string.IsNullOrEmpty(eventEntity.ThumbnailUrl))
            {
                var thumbnailDeleted = await _imageUploadService.DeleteImageAsync(eventEntity.ThumbnailUrl);
                if (thumbnailDeleted)
                {
                    _logger.LogInformation("Deleted thumbnail image for event {EventId}: {ThumbnailUrl}", id, eventEntity.ThumbnailUrl);
                }
                else
                {
                    _logger.LogWarning("Failed to delete thumbnail image for event {EventId}: {ThumbnailUrl}", id, eventEntity.ThumbnailUrl);
                }
            }

            // Delete the event from database
            await _eventService.DeleteEventAsync(id);

            _logger.LogInformation("Event {EventId} deleted successfully along with images", id);

            TempData["SuccessMessage"] = $"Event '{eventName}' has been deleted successfully along with its images.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the event.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Admin/Events/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            var viewModel = new CreateEventViewModel();

            // Load available categories (exclude Undefined - CategoryId = 11)
            var categories = await _categoryRepository.GetAllAsync();
            viewModel.AvailableCategories = categories
                .Where(c => c.Id != 11) // Exclude Undefined category
                .Select(c => new CategoryOption
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .OrderBy(c => c.Name)
                .ToList();

            // Load all subcategories (will be filtered by JavaScript)
            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            viewModel.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId
            }).OrderBy(sc => sc.Name).ToList();

            // Load available tags
            var allTags = await _tagService.GetAllTagsAsync();
            viewModel.AvailableTags = allTags
                .Select(t => new TagOption
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .OrderBy(t => t.Name)
                .ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create event page");
            TempData["ErrorMessage"] = "An error occurred while loading the create event page.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Repopulate available categories and subcategories (exclude Undefined)
                var categories = await _categoryRepository.GetAllAsync();
                model.AvailableCategories = categories
                    .Where(c => c.Id != 11) // Exclude Undefined category
                    .Select(c => new CategoryOption { Id = c.Id, Name = c.Name })
                    .OrderBy(c => c.Name)
                    .ToList();

                var allSubCategories = await _subCategoryRepository.GetAllAsync();
                model.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    CategoryId = sc.CategoryId
                }).OrderBy(sc => sc.Name).ToList();

                var allTags = await _tagService.GetAllTagsAsync();
                model.AvailableTags = allTags
                    .Select(t => new TagOption { Id = t.Id, Name = t.Name })
                    .OrderBy(t => t.Name)
                    .ToList();

                return View(model);
            }

            // Create event entity
            var eventEntity = new Event
            {
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                City = model.City,
                Location = model.Location,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                ThumbnailUrl = model.ThumbnailUrl,
                TicketUrl = model.TicketUrl,
                IsFree = model.IsFree,
                Price = model.Price,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId,
                SubCategoryId = model.SubCategoryId,
                Status = model.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventService.CreateEventAsync(eventEntity);

            // Assign selected tags to the event
            if (model.SelectedTagIds.Any())
            {
                await _tagService.BulkAddTagsToEventAsync(createdEvent.Id, model.SelectedTagIds);
                _logger.LogInformation("Assigned {TagCount} tags to event {EventId}",
                    model.SelectedTagIds.Count, createdEvent.Id);
            }

            _logger.LogInformation("Event {EventId} created successfully by admin", createdEvent.Id);

            TempData["SuccessMessage"] = $"Event '{createdEvent.Name}' has been created successfully.";

            // Redirect to edit page for preview and further modifications
            return RedirectToAction(nameof(Edit), new { id = createdEvent.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            TempData["ErrorMessage"] = "An error occurred while creating the event.";

            // Repopulate dropdowns on error (exclude Undefined)
            var categories = await _categoryRepository.GetAllAsync();
            model.AvailableCategories = categories
                .Where(c => c.Id != 11) // Exclude Undefined category
                .Select(c => new CategoryOption { Id = c.Id, Name = c.Name })
                .OrderBy(c => c.Name)
                .ToList();

            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            model.AvailableSubCategories = allSubCategories.Select(sc => new SubCategoryOption
            {
                Id = sc.Id,
                Name = sc.Name,
                CategoryId = sc.CategoryId
            }).OrderBy(sc => sc.Name).ToList();

            var allTags = await _tagService.GetAllTagsAsync();
            model.AvailableTags = allTags
                .Select(t => new TagOption { Id = t.Id, Name = t.Name })
                .OrderBy(t => t.Name)
                .ToList();

            return View(model);
        }
    }

    // POST: Admin/Events/BulkApplyChanges
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkApplyChanges(BulkEventOperationViewModel model)
    {
        try
        {
            if (model?.SelectedEventIds == null || model.SelectedEventIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No events selected.";
                return RedirectToAction(nameof(Index));
            }

            if (model.OperationsToApply == null || model.OperationsToApply.Count == 0)
            {
                TempData["ErrorMessage"] = "No operations selected.";
                return RedirectToAction(nameof(Index));
            }

            // Batch operation strategy: load all events, modify in memory, update in batch
            // Benefits: Single database transaction, minimal round-trips, better performance
            // Trade-off: Slightly more memory usage (negligible for typical admin operations)

            int changedCount = 0;

            // Phase 1: Load all events and apply structural changes in memory
            var eventsToUpdate = new List<Event>();

            foreach (var eventId in model.SelectedEventIds)
            {
                try
                {
                    var eventEntity = await _eventService.GetEventByIdAsync(eventId);
                    if (eventEntity == null)
                    {
                        continue;
                    }

                    bool eventChanged = false;
                    if (model.OperationsToApply.Contains("category") && model.BulkCategoryId.HasValue)
                    {
                        eventEntity.CategoryId = model.BulkCategoryId.Value;
                        eventChanged = true;
                    }

                    if (model.OperationsToApply.Contains("subcategory") && model.BulkSubCategoryId.HasValue)
                    {
                        eventEntity.SubCategoryId = model.BulkSubCategoryId.Value;
                        eventChanged = true;
                    }

                    if (model.OperationsToApply.Contains("starttime") && model.BulkStartTime.HasValue)
                    {
                        eventEntity.StartTime = model.BulkStartTime.Value;
                        eventChanged = true;
                    }

                    if (model.OperationsToApply.Contains("isfree") && model.BulkIsFree.HasValue)
                    {
                        eventEntity.IsFree = model.BulkIsFree.Value;
                        eventChanged = true;
                    }

                    if (model.OperationsToApply.Contains("status") && model.BulkStatus.HasValue)
                    {
                        eventEntity.Status = model.BulkStatus.Value;
                        eventChanged = true;
                    }

                    if (eventChanged)
                    {
                        eventEntity.UpdatedAt = DateTime.UtcNow;
                        eventsToUpdate.Add(eventEntity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading event {EventId} for bulk update", eventId);
                    continue;
                }
            }

            // Phase 2: Batch update all modified events in single transaction
            if (eventsToUpdate.Any())
            {
                changedCount = await _eventService.BulkUpdateEventsAsync(eventsToUpdate);
            }

            // Phase 3: Batch assign tags (separate operation for better separation of concerns)
            if (model.OperationsToApply.Contains("tags") && model.BulkTagIds.Any())
            {
                // Add selected tags to all events (existing tags are preserved)
                await _tagService.BulkAssignTagsToMultipleEventsAsync(model.SelectedEventIds, model.BulkTagIds);
            }

            _logger.LogInformation(
                "Bulk operations completed: {ChangedCount} events updated, Tags assigned: {HasTags}, Operations: {Operations}",
                changedCount, model.OperationsToApply.Contains("tags"), string.Join(", ", model.OperationsToApply));

            var successMessages = new List<string>();
            if (changedCount > 0)
                successMessages.Add($"{changedCount} event(s) updated");
            if (model.OperationsToApply.Contains("tags") && model.BulkTagIds.Any())
                successMessages.Add($"tags assigned to {model.SelectedEventIds.Count} event(s)");

            TempData["SuccessMessage"] = successMessages.Any()
                ? $"Bulk operations completed successfully: {string.Join(", ", successMessages)}."
                : "Bulk operations completed.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying bulk operations");
            TempData["ErrorMessage"] = "An error occurred while applying bulk operations.";
            return RedirectToAction(nameof(Index));
        }
    }

}
