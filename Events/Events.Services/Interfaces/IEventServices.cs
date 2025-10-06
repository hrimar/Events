using Events.Models.Entities;

namespace Events.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetEventsByCategoryAsync(Events.Models.Enums.EventCategory category);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);
}

public interface ITagService
{
    Task<Tag?> GetTagByIdAsync(int id);
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(Tag tag);
    Task<Tag> UpdateTagAsync(Tag tag);
    Task DeleteTagAsync(int id);
    Task AddTagToEventAsync(int eventId, int tagId);
    Task RemoveTagFromEventAsync(int eventId, int tagId);
}

public interface IAiTaggingService
{
    Task<IEnumerable<string>> GenerateTagsAsync(string eventName, string description, Events.Models.Enums.EventCategory category);
    Task<Events.Models.Enums.EventCategory> ClassifyEventAsync(string eventName, string description);
}

public interface IEventValidationService
{
    Task<bool> ValidateEventAsync(Event eventEntity);
    Task<IEnumerable<string>> GetValidationErrorsAsync(Event eventEntity);
}