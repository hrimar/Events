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
}
