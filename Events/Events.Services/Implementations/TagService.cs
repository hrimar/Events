using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
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
}
