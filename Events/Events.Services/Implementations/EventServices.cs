using Events.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        try
        {
            return await _eventRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event with ID {EventId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        try
        {
            return await _eventRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all events");
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by date range {StartDate} - {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(Events.Models.Enums.EventCategory category)
    {
        try
        {
            return await _eventRepository.GetByCategoryAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by category {Category}", category);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        try
        {
            return await _eventRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<Event> CreateEventAsync(Event eventEntity)
    {
        try
        {
            return await _eventRepository.AddAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event {EventName}", eventEntity.Name);
            throw;
        }
    }

    public async Task<Event> UpdateEventAsync(Event eventEntity)
    {
        try
        {
            return await _eventRepository.UpdateAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", eventEntity.Id);
            throw;
        }
    }

    public async Task DeleteEventAsync(int id)
    {
        try
        {
            await _eventRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw;
        }
    }
}

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<TagService> _logger;

    public TagService(ITagRepository tagRepository, IEventRepository eventRepository, ILogger<TagService> logger)
    {
        _tagRepository = tagRepository;
        _eventRepository = eventRepository;
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
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                throw new ArgumentException($"Tag with ID {tagId} not found");

            if (!eventEntity.EventTags.Any(et => et.TagId == tagId))
            {
                eventEntity.EventTags.Add(new EventTag { EventId = eventId, TagId = tagId });
                await _eventRepository.UpdateAsync(eventEntity);
            }
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
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new ArgumentException($"Event with ID {eventId} not found");

            var eventTag = eventEntity.EventTags.FirstOrDefault(et => et.TagId == tagId);
            if (eventTag != null)
            {
                eventEntity.EventTags.Remove(eventTag);
                await _eventRepository.UpdateAsync(eventEntity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tag {TagId} from event {EventId}", tagId, eventId);
            throw;
        }
    }
}