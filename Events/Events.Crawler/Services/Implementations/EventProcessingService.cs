using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Crawler.Services.Implementations;

public class EventProcessingService : IEventProcessingService
{
    private readonly IEventService _eventService;
    private readonly ITagService _tagService;
    private readonly IAiTaggingService _aiTaggingService;
    private readonly ILogger<EventProcessingService> _logger;

    public EventProcessingService(
        IEventService eventService,
        ITagService tagService,
        IAiTaggingService aiTaggingService,
        ILogger<EventProcessingService> logger)
    {
        _eventService = eventService;
        _tagService = tagService;
        _aiTaggingService = aiTaggingService;
        _logger = logger;
    }

    public async Task<ProcessingResult> ProcessCrawledEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        var result = new ProcessingResult();
        
        foreach (var crawledEvent in crawledEvents)
        {
            try
            {
                // Check if event already exists
                var existingEvent = await FindExistingEventAsync(crawledEvent);
                
                if (existingEvent != null)
                {
                    // Update existing event if needed
                    var updatedEvent = await UpdateEventFromCrawledDataAsync(existingEvent, crawledEvent);
                    if (updatedEvent != null)
                    {
                        result.EventsUpdated++;
                        result.ProcessedEventIds.Add(updatedEvent.Id);
                    }
                    else
                    {
                        result.EventsSkipped++;
                    }
                }
                else
                {
                    // Create new event
                    var newEvent = await MapToEntityAsync(crawledEvent);
                    var createdEvent = await _eventService.CreateEventAsync(newEvent);
                    result.EventsCreated++;
                    result.ProcessedEventIds.Add(createdEvent.Id);
                }
                
                result.EventsProcessed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing crawled event: {EventName} from {Source}", 
                    crawledEvent.Name, crawledEvent.Source);
                result.Errors.Add($"Error processing event '{crawledEvent.Name}': {ex.Message}");
            }
        }

        return result;
    }

    public async Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        // First, process and save events
        var processingResult = await ProcessCrawledEventsAsync(crawledEvents);
        
        // Then, tag the successfully processed events
        if (processingResult.ProcessedEventIds.Any())
        {
            try
            {
                await TagEventsAsync(processingResult.ProcessedEventIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during tagging process");
                processingResult.Errors.Add($"Tagging error: {ex.Message}");
            }
        }

        return processingResult;
    }

    public async Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent)
    {
        try
        {
            // Search by external ID first (if we have it stored)
            var allEvents = await _eventService.GetAllEventsAsync();
            
            // Look for events with matching source URL or name/date combination
            var potentialMatches = allEvents.Where(e => 
                //(!string.IsNullOrEmpty(e.SourceUrl) && e.SourceUrl == crawledEvent.SourceUrl) ||
                (e.Name.Equals(crawledEvent.Name, StringComparison.OrdinalIgnoreCase) && 
                 crawledEvent.StartDate.HasValue && 
                 e.Date.Date == crawledEvent.StartDate.Value.Date));

            return potentialMatches.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding existing event for: {EventName}", crawledEvent.Name);
            return null;
        }
    }

    public async Task<Event> MapToEntityAsync(CrawledEventDto crawledEvent)
    {
        try
        {
            EventCategory? category = null;

            // AI clasification only if we have both name and description
            if (!string.IsNullOrEmpty(crawledEvent.Name) && !string.IsNullOrEmpty(crawledEvent.Description))
            {
                try
                {
                    category = await _aiTaggingService.ClassifyEventAsync(crawledEvent.Name, crawledEvent.Description);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI classification failed for event {EventName}", crawledEvent.Name);
                }
            }

            if (category == null)
            {
                _logger.LogInformation("Event '{EventName}' will be created as Draft for manual categorization", crawledEvent.Name);
            }

            var eventEntity = new Event
            {
                Name = TruncateString(crawledEvent.Name, 200),
                Description = TruncateString(crawledEvent.Description, 2000),
                Date = crawledEvent.StartDate ?? DateTime.Now.AddDays(1),
                StartTime = crawledEvent.StartDate?.TimeOfDay,
                Location = TruncateString(crawledEvent.Location ?? "TBD", 300), // TODO: Clarify missed locations in bilet.bg
                ImageUrl = TruncateString(crawledEvent.ImageUrl, 500),
                TicketUrl = TruncateString(crawledEvent.TicketUrl, 500),
                SourceUrl = TruncateString(crawledEvent.SourceUrl, 500),
                Price = crawledEvent.Price,
                IsFree = crawledEvent.IsFree || crawledEvent.Price == 0,
                Category = category,
                Status = category.HasValue ? EventStatus.Published : EventStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return eventEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping crawled event to entity: {EventName}", crawledEvent.Name);
            throw;
        }
    }

    public async Task TagEventsAsync(IEnumerable<int> eventIds)
    {
        foreach (var eventId in eventIds)
        {
            try
            {
                var eventEntity = await _eventService.GetEventByIdAsync(eventId);
                if (eventEntity == null) continue;

                // Generate tags using AI
                var taggingResult = await _aiTaggingService.GenerateTagsAsync(
                    eventEntity.Name, 
                    eventEntity.Description ?? "", 
                    eventEntity.Location);

                // Create or get existing tags
                foreach (var tagName in taggingResult.SuggestedTags)
                {
                    try
                    {
                        var existingTag = await _tagService.GetTagByNameAsync(tagName);
                        if (existingTag == null)
                        {
                            var newTag = new Tag
                            {
                                Name = tagName,
                                Category = eventEntity.Category,
                                CreatedAt = DateTime.UtcNow
                            };
                            existingTag = await _tagService.CreateTagAsync(newTag);
                        }

                        // Add tag to event
                        await _tagService.AddTagToEventAsync(eventId, existingTag.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to add tag '{TagName}' to event {EventId}", tagName, eventId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tagging event {EventId}", eventId);
            }
        }
    }

    private async Task<Event?> UpdateEventFromCrawledDataAsync(Event existingEvent, CrawledEventDto crawledEvent)
    {
        var needsUpdate = false;

        // Check if any significant data has changed
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
            return await _eventService.UpdateEventAsync(existingEvent);
        }

        return null; // No update needed
    }

    private static string TruncateString(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Length <= maxLength ? input : input[..maxLength];
    }
}