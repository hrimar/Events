using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Events.Services.Models.Admin.DTOs;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventTagRepository _eventTagRepository;
    private readonly ILogger<TagService> _logger;

    public TagService(
        ITagRepository tagRepository,
        IEventRepository eventRepository,
        IEventTagRepository eventTagRepository,
        ILogger<TagService> logger)
    {
        _tagRepository = tagRepository;
        _eventRepository = eventRepository;
        _eventTagRepository = eventTagRepository;
        _logger = logger;
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        try
        {
            return await _tagRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag with ID {TagId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        try
        {
            return await _tagRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tags");
            throw;
        }
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        try
        {
            return await _tagRepository.GetByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag by name {TagName}", name);
            throw;
        }
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        try
        {
            return await _tagRepository.AddAsync(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag {TagName}", tag.Name);
            throw;
        }
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        try
        {
            return await _tagRepository.UpdateAsync(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag {TagId}", tag.Id);
            throw;
        }
    }

    public async Task DeleteTagAsync(int id)
    {
        try
        {
            await _tagRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag {TagId}", id);
            throw;
        }
    }

    public async Task AddTagToEventAsync(int eventId, int tagId)
    {
        try
        {
            // Check if already exists to avoid duplicates
            if (await _eventTagRepository.EventTagExistsAsync(eventId, tagId))
            {
                return; // Already exists, no need to add
            }

            var eventTag = new EventTag { EventId = eventId, TagId = tagId };
            await _eventTagRepository.BulkAddEventTagsAsync(new List<EventTag> { eventTag });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tag {TagId} to event {EventId}", tagId, eventId);
            throw;
        }
    }

    public async Task RemoveTagFromEventAsync(int eventId, int tagId)
    {
        try
        {
            // Get all event tags and remove the specific one
            var eventTags = await _eventTagRepository.GetEventTagsByEventIdAsync(eventId);
            var tagToRemove = eventTags.FirstOrDefault(et => et.TagId == tagId);
            
            if (tagToRemove != null)
            {
                // For single removal, we use direct EF operations
                var eventEntity = await _eventRepository.GetByIdAsync(eventId);
                if (eventEntity != null)
                {
                    var eventTagToRemove = eventEntity.EventTags.FirstOrDefault(et => et.TagId == tagId);
                    if (eventTagToRemove != null)
                    {
                        eventEntity.EventTags.Remove(eventTagToRemove);
                        await _eventRepository.UpdateAsync(eventEntity);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tag {TagId} from event {EventId}", tagId, eventId);
            throw;
        }
    }

    public async Task<AdminTagListResult> GetAdminTagsAsync(AdminTagQuery query, CancellationToken cancellationToken = default)
    {
        var normalized = (query ?? new AdminTagQuery()).Normalize();

        try
        {
            var (tagProjections, totalCount) = await _tagRepository.GetPagedAdminTagsAsync(
                normalized.Page,
                normalized.PageSize,
                normalized.SearchTerm,
                normalized.Category,
                normalized.ShowOrphansOnly,
                normalized.ShowWithoutCategoryOnly,
                normalized.SortBy,
                normalized.SortOrder,
                cancellationToken);

            var tagIds = tagProjections.Select(p => p.Id).ToList();
            var usageLookup = await _tagRepository.GetUsageAggregatesAsync(tagIds, cancellationToken);

            var dtoList = tagProjections.Select(p =>
            {
                usageLookup.TryGetValue(p.Id, out var aggregate);
                return new AdminTagDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    CreatedAt = p.CreatedAt,
                    UsageCount = aggregate?.UsageCount ?? p.UsageCount,
                    CategoryUsage = aggregate?.Categories ?? Array.Empty<string>(),
                    SubCategoryUsage = aggregate?.SubCategories ?? Array.Empty<string>()
                };
            }).ToList();

            var statisticsResult = await _tagRepository.GetStatisticsAsync(cancellationToken);

            return new AdminTagListResult
            {
                Tags = dtoList,
                Page = normalized.Page,
                PageSize = normalized.PageSize,
                TotalCount = totalCount,
                SearchTerm = normalized.SearchTerm,
                Category = normalized.Category,
                ShowOrphansOnly = normalized.ShowOrphansOnly,
                ShowWithoutCategoryOnly = normalized.ShowWithoutCategoryOnly,
                SortBy = normalized.SortBy,
                SortOrder = normalized.SortOrder,
                Statistics = new AdminTagStatisticsDto
                {
                    TotalTags = statisticsResult.TotalTags,
                    WithoutCategoryTags = statisticsResult.WithoutCategoryTags,
                    OrphanTags = statisticsResult.OrphanTags,
                    MostUsedTagName = statisticsResult.MostUsedTagName,
                    MostUsedTagCount = statisticsResult.MostUsedTagCount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin tags for page {Page}", normalized.Page);
            throw;
        }
    }

    // Bulk operations for better performance
    public async Task BulkAddTagsToEventAsync(int eventId, List<int> tagIds)
    {
        try
        {
            if (!tagIds.Any()) return;

            var eventTags = tagIds.Select(tagId => new EventTag
            {
                EventId = eventId,
                TagId = tagId
            }).ToList();

            await _eventTagRepository.BulkAddEventTagsAsync(eventTags);
            
            _logger.LogInformation("Bulk added {Count} tags to event {EventId}", tagIds.Count, eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk adding tags to event {EventId}", eventId);
            throw;
        }
    }

    public async Task BulkRemoveTagsFromEventAsync(int eventId)
    {
        try
        {
            await _eventTagRepository.BulkRemoveEventTagsByEventIdAsync(eventId);
            _logger.LogInformation("Bulk removed all tags from event {EventId}", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk removing tags from event {EventId}", eventId);
            throw;
        }
    }

    public async Task<Dictionary<int, List<string>>> GetEventTagsBulkAsync(List<int> eventIds)
    {
        try
        {
            return await _eventTagRepository.GetEventTagsBulkAsync(eventIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bulk event tags for {Count} events", eventIds.Count);
            throw;
        }
    }

    public async Task BulkAssignTagsToMultipleEventsAsync(IEnumerable<int> eventIds, IEnumerable<int> tagIds)
    {
        try
        {
            if (eventIds == null || !eventIds.Any() || tagIds == null || !tagIds.Any())
            {
                _logger.LogWarning("BulkAssignTagsToMultipleEventsAsync called with empty eventIds or tagIds");
                return;
            }

            var eventIdList = eventIds.ToList();
            var tagIdList = tagIds.ToList();

            var eventTags = new List<EventTag>();

            foreach (var eventId in eventIdList)
            {
                // Check if event exists
                if (!await _eventRepository.ExistsAsync(eventId))
                {
                    _logger.LogWarning("Event with ID {EventId} not found during bulk tag assignment", eventId);
                    continue;
                }

                foreach (var tagId in tagIdList)
                {
                    // Check if tag already exists for this event to avoid duplicates
                    if (!await _eventTagRepository.EventTagExistsAsync(eventId, tagId))
                    {
                        eventTags.Add(new EventTag
                        {
                            EventId = eventId,
                            TagId = tagId
                        });
                    }
                }
            }

            if (eventTags.Any())
            {
                await _eventTagRepository.BulkAddEventTagsAsync(eventTags);

                _logger.LogInformation(
                    "Bulk assigned {TagCount} tags to {EventCount} events ({TotalOperations} total operations)",
                    tagIdList.Count,
                    eventIdList.Count,
                    eventTags.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk tag assignment to multiple events");
            throw new ApplicationException("Failed to bulk assign tags to events", ex);
        }
    }
}
