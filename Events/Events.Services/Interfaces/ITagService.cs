using Events.Models.Entities;

namespace Events.Services.Interfaces;

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
    
    // Bulk operations for better performance
    Task BulkAddTagsToEventAsync(int eventId, List<int> tagIds);
    Task BulkRemoveTagsFromEventAsync(int eventId);
    Task<Dictionary<int, List<string>>> GetEventTagsBulkAsync(List<int> eventIds);
}
