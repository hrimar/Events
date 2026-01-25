using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Crawler.Services;
using Events.Crawler.Services.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Helpers;
using Events.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Events.Crawler.Services.Implementations;

public class EventProcessingService : IEventProcessingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAiTaggingService _aiTaggingService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly ILogger<EventProcessingService> _logger;

    public EventProcessingService(
        IServiceProvider serviceProvider,
        IAiTaggingService aiTaggingService,
        ISubCategoryService subCategoryService,
        ILogger<EventProcessingService> logger)
    {
        _serviceProvider = serviceProvider;
        _aiTaggingService = aiTaggingService;
        _subCategoryService = subCategoryService;
        _logger = logger;
    }

    public async Task<ProcessingResult> ProcessCrawledEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        var result = new ProcessingResult();

        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        foreach (var crawledEvent in crawledEvents)
        {
            try
            {
                var existingEvent = await FindExistingEventAsync(crawledEvent, eventService);

                if (existingEvent != null)
                {
                    // TODO: Do not update existing events or update only existing events not older than today by Date or other!!!
                    result.EventsSkipped++;
                    //var updatedEvent = await UpdateEventFromCrawledDataAsync(existingEvent, crawledEvent, eventService);
                    //if (updatedEvent != null)
                    //{
                    //    result.EventsUpdated++;
                    //    result.ProcessedEventIds.Add(updatedEvent.Id);
                    //}
                    //else
                    //{
                    //    result.EventsSkipped++;
                    //}
                }
                else
                {
                    // Use comprehensive processing with both categorization AND tagging in one go
                    var newEvent = await CreateEventEntityComprehensivelyAsync(crawledEvent);
                    var createdEvent = await eventService.CreateEventAsync(newEvent.Event);
                    
                    // Use bulk tag assignment with proper category setting
                    if (newEvent.Tags.Any())
                    {
                        using var tagScope = _serviceProvider.CreateScope();
                        var tagService = tagScope.ServiceProvider.GetRequiredService<ITagService>();
                        
                        // Pass event category to properly set tag categories
                        var eventCategory = newEvent.Event.CategoryId != 11 ? (EventCategory)newEvent.Event.CategoryId : (EventCategory?)null;
                        await BulkAssignTagsToEventAsync(createdEvent.Id, newEvent.Tags, tagService, eventCategory);
                    }

                    result.EventsCreated++;
                    result.ProcessedEventIds.Add(createdEvent.Id);
                }

                result.EventsProcessed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing crawled event: {EventName} from {Source}", crawledEvent.Name, crawledEvent.Source);
                result.Errors.Add($"Error processing event '{crawledEvent.Name}': {ex.Message}");
            }
        }

        return result;
    }

    public async Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        // No longer need separate tagging step - everything is in ProcessCrawledEventsAsync
        var processingResult = await ProcessCrawledEventsAsync(crawledEvents);

        _logger.LogInformation("Processed {Count} events with comprehensive AI tagging", processingResult.EventsProcessed);

        return processingResult;
    }

    public async Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        return await FindExistingEventAsync(crawledEvent, eventService);
    }

    private async Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent, IEventService eventService)
    {
        try
        {
            if (crawledEvent.StartDate.HasValue)
            {
                var eventDate = crawledEvent.StartDate.Value.Date;
                var events = await eventService.GetEventsByDateRangeAsync(eventDate, eventDate.AddDays(1));

                return events.FirstOrDefault(e => e.Name.Equals(crawledEvent.Name, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding existing event for: {EventName}", crawledEvent.Name);
            return null;
        }
    }

    private async Task<int?> GetSubCategoryIdAsync(EventCategory category, string? subCategoryName)
    {
        if (string.IsNullOrWhiteSpace(subCategoryName))
        {
            return null;
        }

        try
        {
            var enumValue = SubCategoryMapper.MapSubCategoryToEnumValue(category, subCategoryName, _logger);

            if (enumValue.HasValue)
            {
                var subCategory = await _subCategoryService.GetByEnumValueAsync(category, enumValue.Value);
                return subCategory?.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting SubCategory ID for {Category}/{SubCategoryName}", category, subCategoryName);
            return null;
        }
    }

    // Comprehensive event creation with single AI call
    private async Task<(Event Event, List<string> Tags)> CreateEventEntityComprehensivelyAsync(CrawledEventDto crawledEvent)
    {
        if (string.IsNullOrEmpty(crawledEvent.Name))
        {
            var fallbackEvent = MapToEntity(crawledEvent, null, null);
            return (fallbackEvent, new List<string>());
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            // SINGLE AI CALL for category + subcategory + tags
            var comprehensiveResult = await _aiTaggingService.ProcessEventComprehensivelyAsync(
                crawledEvent.Name,
                crawledEvent.Description ?? "",
                crawledEvent.Location).WaitAsync(cts.Token);

            var category = comprehensiveResult.SuggestedCategory;
            var suggestedSubCategory = comprehensiveResult.SuggestedSubCategory;
            var tags = comprehensiveResult.SuggestedTags;

            int? subCategoryId = null;
            if (category.HasValue && !string.IsNullOrEmpty(suggestedSubCategory))
            {
                subCategoryId = await GetSubCategoryIdAsync(category.Value, suggestedSubCategory);
            }

            var eventEntity = MapToEntity(crawledEvent, category, subCategoryId);

            _logger.LogInformation("Comprehensively processed '{EventName}': Category={Category}, SubCategory={SubCategory}, Tags=[{Tags}]",
                crawledEvent.Name, category?.ToString() ?? "None", suggestedSubCategory ?? "None", string.Join(", ", tags));

            return (eventEntity, tags);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AI comprehensive processing timeout for event {EventName}, using fallback", crawledEvent.Name);
            var fallbackEvent = MapToEntity(crawledEvent, null, null);
            return (fallbackEvent, new List<string> { "некласифицирано" });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI comprehensive processing failed for event {EventName}, using fallback", crawledEvent.Name);
            var fallbackEvent = MapToEntity(crawledEvent, null, null);
            return (fallbackEvent, new List<string> { "некласифицирано" });
        }
    }

    private Event MapToEntity(CrawledEventDto crawledEvent, EventCategory? category, int? subCategoryId)
    {
        var categoryId = category.HasValue ? (int)category.Value : 11; // Default to Undefined

        return new Event
        {
            Name = TruncateString(crawledEvent.Name, 200),
            Description = TruncateString(crawledEvent.Description, 2000),
            Date = crawledEvent.StartDate ?? DateTime.MinValue, // TODO: Handle missing date better
            StartTime = crawledEvent.StartDate?.TimeOfDay,
            City = TruncateString(crawledEvent.City, 100),
            Location = TruncateString(crawledEvent.Location ?? "", 300),
            ImageUrl = TruncateString(crawledEvent.ImageUrl, 500),
            TicketUrl = TruncateString(crawledEvent.TicketUrl, 500),
            SourceUrl = TruncateString(crawledEvent.SourceUrl, 500),
            Price = crawledEvent.Price,
            IsFree = crawledEvent.IsFree || crawledEvent.Price == 0,
            CategoryId = categoryId,
            SubCategoryId = subCategoryId,
            Status = category.HasValue ? EventStatus.Published : EventStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public async Task<Event> CreateEventEntityAsync(CrawledEventDto crawledEvent)
    {
        var result = await CreateEventEntityComprehensivelyAsync(crawledEvent);
        return result.Event;
    }

    // Bulk tag assignment with proper category setting
    private async Task BulkAssignTagsToEventAsync(int eventId, List<string> tags, ITagService tagService, EventCategory? eventCategory)
    {
        if (!tags.Any()) return;

        var tagIds = new List<int>();
        
        foreach (var tagName in tags)
        {
            var cleanTagName = TagNameNormalizer.Normalize(tagName);
            if (string.IsNullOrEmpty(cleanTagName)) continue;

            try
            {
                var existingTag = await tagService.GetTagByNameAsync(cleanTagName);
                if (existingTag == null)
                {
                    // Properly set the tag category based on event category
                    var newTag = new Tag
                    {
                        Name = cleanTagName,
                        Category = eventCategory, // Set based on event category
                        CreatedAt = DateTime.UtcNow
                    };
                    existingTag = await tagService.CreateTagAsync(newTag);
                    
                    _logger.LogInformation("Created new tag '{TagName}' with category {Category}", cleanTagName, eventCategory?.ToString() ?? "None");
                }
                else
                {
                    // Update existing tag category if it's null but event has category
                    if (existingTag.Category == null && eventCategory.HasValue)
                    {
                        existingTag.Category = eventCategory.Value;
                        await tagService.UpdateTagAsync(existingTag);
                        _logger.LogInformation("Updated tag '{TagName}' category to {Category}", cleanTagName, eventCategory);
                    }
                }

                tagIds.Add(existingTag.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process tag '{TagName}' for event {EventId}", cleanTagName, eventId);
            }
        }

        // Bulk insert - a single database operation instead of N operations
        if (tagIds.Any())
        {
            try
            {
                await tagService.BulkAddTagsToEventAsync(eventId, tagIds);
                _logger.LogInformation("Bulk assigned {Count} tags to event {EventId} with category {Category}", 
                    tagIds.Count, eventId, eventCategory?.ToString() ?? "None");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to bulk assign tags to event {EventId}", eventId);
            }
        }
    }

    private async Task<Event?> UpdateEventFromCrawledDataAsync(Event existingEvent, CrawledEventDto crawledEvent, IEventService eventService)
    {
        var needsUpdate = false;

        if (!string.IsNullOrEmpty(crawledEvent.Description) &&
            existingEvent.Description != crawledEvent.Description)
        {
            existingEvent.Description = TruncateString(crawledEvent.Description, 2000);
            needsUpdate = true;
        }

        if (!string.IsNullOrEmpty(crawledEvent.ImageUrl) &&
            existingEvent.ImageUrl != crawledEvent.ImageUrl)
        {
            existingEvent.ImageUrl = TruncateString(crawledEvent.ImageUrl, 500);
            needsUpdate = true;
        }

        if (crawledEvent.Price.HasValue && existingEvent.Price != crawledEvent.Price)
        {
            existingEvent.Price = crawledEvent.Price;
            existingEvent.IsFree = crawledEvent.Price == 0;
            needsUpdate = true;
        }

        if (needsUpdate)
        {
            existingEvent.UpdatedAt = DateTime.UtcNow;
            return await eventService.UpdateEventAsync(existingEvent);
        }

        return null; // No update needed
    }

    private static string TruncateString(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Length <= maxLength ? input : input[..maxLength];
    }
}