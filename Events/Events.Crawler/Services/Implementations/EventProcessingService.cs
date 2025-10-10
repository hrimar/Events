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
    private readonly ISubCategoryService _subCategoryService;
    private readonly ILogger<EventProcessingService> _logger;

    public EventProcessingService(
        IEventService eventService,
        ITagService tagService,
        IAiTaggingService aiTaggingService,
        ISubCategoryService subCategoryService,
        ILogger<EventProcessingService> logger)
    {
        _eventService = eventService;
        _tagService = tagService;
        _aiTaggingService = aiTaggingService;
        _subCategoryService = subCategoryService;
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
            int? categoryId = null;
            int? subCategoryId = null;

            if (!string.IsNullOrEmpty(crawledEvent.Name) && !string.IsNullOrEmpty(crawledEvent.Description))
            {
                try
                {
                    // Get AI suggestions
                    var taggingResult = await _aiTaggingService.GenerateTagsAsync(
                        crawledEvent.Name,
                        crawledEvent.Description,
                        crawledEvent.Location);

                    category = taggingResult.SuggestedCategory;

                    if (category.HasValue)
                    {
                        categoryId = (int)category.Value;
                    }

                    // If we have both category and subcategory suggestion
                    if (category.HasValue && !string.IsNullOrEmpty(taggingResult.SuggestedSubCategory))
                    {
                        subCategoryId = await GetSubCategoryIdAsync(category.Value, taggingResult.SuggestedSubCategory);
                    }
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
                Location = TruncateString(crawledEvent.Location ?? "TBD", 300),
                ImageUrl = TruncateString(crawledEvent.ImageUrl, 500),
                TicketUrl = TruncateString(crawledEvent.TicketUrl, 500),
                SourceUrl = TruncateString(crawledEvent.SourceUrl, 500),
                Price = crawledEvent.Price,
                IsFree = crawledEvent.IsFree || crawledEvent.Price == 0,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId, // Set subcategory
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
                    // TODO: Prevent creating empty or white space tags
                    try
                    {
                        var existingTag = await _tagService.GetTagByNameAsync(tagName);
                        if (existingTag == null)
                        {
                            var newTag = new Tag
                            {
                                Name = tagName,
                                Category = eventEntity.CategoryId.HasValue ? (EventCategory?)eventEntity.CategoryId.Value : null,
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

    private async Task<int?> GetSubCategoryIdAsync(EventCategory category, string subCategoryName)
    {
        try
        {
            // Try to find existing subcategory by name
            var subCategory = await _subCategoryService.GetByNameAsync(category, subCategoryName);
            if (subCategory != null)
            {
                return subCategory.Id;
            }

            // Try to map AI suggestion to enum values
            var enumValue = MapSubCategoryNameToEnum(category, subCategoryName);
            if (enumValue.HasValue)
            {
                subCategory = await _subCategoryService.GetByEnumValueAsync(category, enumValue.Value);
                return subCategory?.Id;
            }

            _logger.LogWarning("Could not find subcategory '{SubCategory}' for category '{Category}'",
                subCategoryName, category);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategory ID for '{SubCategory}' in category '{Category}'",
                subCategoryName, category);
            return null;
        }
    }

    private static int? MapSubCategoryNameToEnum(EventCategory category, string subCategoryName)
    {
        var lowerName = subCategoryName.ToLower();

        return category switch
        {
            EventCategory.Music => MapMusicSubCategory(lowerName),
            EventCategory.Sports => MapSportsSubCategory(lowerName),
            _ => null
        };
    }

    private static int? MapMusicSubCategory(string name)
    {
        return name switch
        {
            "rock" or "рок" => (int)MusicSubCategory.Rock,
            "jazz" or "джаз" => (int)MusicSubCategory.Jazz,
            "metal" or "метъл" or "heavy metal" or "thrash metal" => (int)MusicSubCategory.Metal,
            "pop" or "поп" => (int)MusicSubCategory.Pop,
            "classical" or "класическа" or "класическа музика" => (int)MusicSubCategory.Classical,
            "electronic" or "електронна" or "techno" or "house" => (int)MusicSubCategory.Electronic,
            "folk" or "фолк" or "народна" => (int)MusicSubCategory.Folk,
            "blues" or "блус" => (int)MusicSubCategory.Blues,
            "hip-hop" or "хип-хоп" or "rap" or "рап" => (int)MusicSubCategory.HipHop,
            "punk" or "пънк" => (int)MusicSubCategory.Punk,
            "funk" or "фънк" => (int)MusicSubCategory.Funk,
            "opera" or "опера" => (int)MusicSubCategory.Opera,
            "country" or "кънтри" => (int)MusicSubCategory.Country,
            "reggae" or "регей" => (int)MusicSubCategory.Reggae,
            "alternative" or "алтернативна" => (int)MusicSubCategory.Alternative,
            _ => null
        };
    }

    private static int? MapSportsSubCategory(string name)
    {
        return name switch
        {
            "football" or "футбол" => (int)SportsSubCategory.Football,
            "basketball" or "баскетбол" => (int)SportsSubCategory.Basketball,
            "tennis" or "тенис" => (int)SportsSubCategory.Tennis,
            "volleyball" or "волейбол" => (int)SportsSubCategory.Volleyball,
            "swimming" or "плуване" => (int)SportsSubCategory.Swimming,
            "athletics" or "атлетика" => (int)SportsSubCategory.Athletics,
            "boxing" or "бокс" => (int)SportsSubCategory.Boxing,
            "wrestling" or "борба" => (int)SportsSubCategory.Wrestling,
            "gymnastics" or "гимнастика" => (int)SportsSubCategory.Gymnastics,
            "cycling" or "колоездене" => (int)SportsSubCategory.Cycling,
            _ => null
        };
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