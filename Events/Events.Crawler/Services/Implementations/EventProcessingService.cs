using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
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

        foreach (var crawledEvent in crawledEvents)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

                var existingEvent = await FindExistingEventAsync(crawledEvent, eventService);

                if (existingEvent != null)
                {
                    var updatedEvent = await UpdateEventFromCrawledDataAsync(existingEvent, crawledEvent, eventService);
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
                    var newEvent = await MapToEntityAsync(crawledEvent);
                    var createdEvent = await eventService.CreateEventAsync(newEvent);
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
        var processingResult = await ProcessCrawledEventsAsync(crawledEvents);

        if (processingResult.ProcessedEventIds.Any())
        {
            try
            {
                await TagEventsBatchAsync(processingResult.ProcessedEventIds);
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

                return events.FirstOrDefault(e =>
                    e.Name.Equals(crawledEvent.Name, StringComparison.OrdinalIgnoreCase));
            }

            return null;
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

            if (!string.IsNullOrEmpty(crawledEvent.Name))
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var taggingTask = _aiTaggingService.GenerateTagsAsync(
                        crawledEvent.Name,
                        crawledEvent.Description ?? "",
                        crawledEvent.Location);

                    var taggingResult = await taggingTask.WaitAsync(cts.Token);
                    category = taggingResult.SuggestedCategory;

                    if (category.HasValue)
                    {
                        categoryId = (int)category.Value;

                        if (!string.IsNullOrEmpty(taggingResult.SuggestedSubCategory))
                        {
                            subCategoryId = await GetSubCategoryIdAsync(category.Value, taggingResult.SuggestedSubCategory);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("AI classification timeout for event {EventName}, skipping AI", crawledEvent.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI classification failed for event {EventName}", crawledEvent.Name);
                }
            }

            var eventEntity = new Event
            {
                Name = TruncateString(crawledEvent.Name, 200),
                Description = TruncateString(crawledEvent.Description, 2000),
                Date = crawledEvent.StartDate ?? DateTime.Now.AddDays(1), // TODO: Handle missing date better
                StartTime = crawledEvent.StartDate?.TimeOfDay,
                Location = TruncateString(crawledEvent.Location ?? "TBD", 300),
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

            return eventEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping crawled event to entity: {EventName}", crawledEvent.Name);
            throw;
        }
    }

    private async Task TagEventsBatchAsync(IEnumerable<int> eventIds)
    {
        const int batchSize = 5;
        var eventIdsList = eventIds.ToList();

        for (int i = 0; i < eventIdsList.Count; i += batchSize)
        {
            var batch = eventIdsList.Skip(i).Take(batchSize);

            foreach (var eventId in batch)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                    var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();

                    await TagSingleEventWithScopedServicesAsync(eventId, eventService, tagService);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error tagging event {EventId}", eventId);
                }
            }

            await Task.Delay(200);
        }
    }

    private async Task TagSingleEventWithScopedServicesAsync(int eventId, IEventService eventService, ITagService tagService)
    {
        var eventEntity = await eventService.GetEventByIdAsync(eventId);
        if (eventEntity == null) return;

        try
        {
            var taggingResult = await _aiTaggingService.GenerateTagsAsync(
                eventEntity.Name,
                eventEntity.Description ?? "",
                eventEntity.Location);

            foreach (var tagName in taggingResult.SuggestedTags)
            {
                var cleanTagName = CleanAndValidateTagName(tagName);
                if (string.IsNullOrEmpty(cleanTagName)) continue;

                await CreateOrAssignTagWithScopedServicesAsync(eventId, cleanTagName, eventEntity.CategoryId, tagService);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TagSingleEventWithScopedServicesAsync for event {EventId}", eventId);
        }
    }

    private async Task CreateOrAssignTagWithScopedServicesAsync(int eventId, string tagName, int? categoryId, ITagService tagService)
    {
        try
        {
            var existingTag = await tagService.GetTagByNameAsync(tagName);
            if (existingTag == null)
            {
                var newTag = new Tag
                {
                    Name = tagName,
                    Category = categoryId.HasValue ? (EventCategory?)categoryId.Value : null,
                    CreatedAt = DateTime.UtcNow
                };
                existingTag = await tagService.CreateTagAsync(newTag);
            }

            await tagService.AddTagToEventAsync(eventId, existingTag.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create/assign tag '{TagName}' to event {EventId}", tagName, eventId);
        }
    }

    private static string? CleanAndValidateTagName(string? tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return null;

        var cleaned = tagName.Trim()
            .Replace("\n", " ")
            .Replace("\r", "")
            .Replace("  ", " ");

        if (cleaned.Length > 100)
        {
            cleaned = cleaned[..97] + "...";
        }

        return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
    }

    private async Task<int?> GetSubCategoryIdAsync(EventCategory category, string subCategoryName)
    {
        try
        {
            var subCategory = await _subCategoryService.GetByNameAsync(category, subCategoryName);
            if (subCategory != null)
            {
                return subCategory.Id;
            }

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