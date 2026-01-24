using Events.Models.Entities;
using Events.Services.Models.Admin;
using Events.Services.Models.Admin.DTOs;

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

    Task<AdminTagListResult> GetAdminTagsAsync(AdminTagQuery query, CancellationToken cancellationToken = default);

    Task DeleteTagsAsync(IEnumerable<int> tagIds);
    Task<int> DeleteOrphanTagsAsync();

    // Bulk operations for better performance
    Task BulkAddTagsToEventAsync(int eventId, List<int> tagIds);
    Task BulkRemoveTagsFromEventAsync(int eventId);
    Task<Dictionary<int, List<string>>> GetEventTagsBulkAsync(List<int> eventIds);

    /// <summary>
    /// Batch assign tags to multiple events efficiently in a single database transaction.
    /// Advantages of batch operations:
    /// - Single database round-trip (not N*M round-trips for N events and M tags)
    /// - One transaction scope ensures consistency across all events
    /// - Optimal for admin operations affecting 5-100 events with multiple tags
    /// - Significantly better performance than sequential individual assignments
    /// </summary>
    Task BulkAssignTagsToMultipleEventsAsync(IEnumerable<int> eventIds, IEnumerable<int> tagIds);
}
