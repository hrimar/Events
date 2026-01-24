using System.Collections.Generic;
using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface IEventTagRepository
{
    Task BulkAddEventTagsAsync(List<EventTag> eventTags);
    Task BulkRemoveEventTagsByEventIdAsync(int eventId);
    Task<Dictionary<int, List<string>>> GetEventTagsBulkAsync(List<int> eventIds);
    Task<List<EventTag>> GetEventTagsByEventIdAsync(int eventId);
    Task<bool> EventTagExistsAsync(int eventId, int tagId);
    Task RemoveEventTagsByTagIdsAsync(IEnumerable<int> tagIds);
}