using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Repositories.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetByCategoryAsync(EventCategory? category);
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task DeleteAsync(int id);
    Task DeleteRangeAsync(IEnumerable<int> ids);

    Task<(List<TagAdminProjection> Tags, int TotalCount)> GetPagedAdminTagsAsync(
        int page,
        int pageSize,
        string? searchTerm,
        EventCategory? category,
        bool showOrphansOnly,
        bool showWithoutCategoryOnly,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken);

    Task<Dictionary<int, TagUsageAggregateResult>> GetUsageAggregatesAsync(
        IReadOnlyCollection<int> tagIds,
        CancellationToken cancellationToken);

    Task<TagStatisticsResult> GetStatisticsAsync(CancellationToken cancellationToken);

    Task<List<int>> GetOrphanTagIdsAsync();

    /// <summary>
    /// Returns tags with their usage count computed via a single aggregate SQL query
    /// (COUNT per tag, filtered to published events on/after fromDate), instead of
    /// loading the full Events/Tags graphs into memory to count matches in code.
    /// </summary>
    Task<List<TagPopularityProjection>> GetPopularTagsAsync(
        DateTime fromDate,
        string? nameFilter = null,
        EventCategory? category = null,
        int? maxCount = null,
        CancellationToken cancellationToken = default);
}

public class TagAdminProjection
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventCategory? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UsageCount { get; set; }
}

public class TagUsageAggregateResult
{
    public TagUsageAggregateResult()
    {
        Categories = Array.Empty<string>();
        SubCategories = Array.Empty<string>();
    }

    public TagUsageAggregateResult(int usageCount, IReadOnlyList<string> categories, IReadOnlyList<string> subCategories)
    {
        UsageCount = usageCount;
        Categories = categories;
        SubCategories = subCategories;
    }

    public int UsageCount { get; set; }
    public IReadOnlyList<string> Categories { get; set; }
    public IReadOnlyList<string> SubCategories { get; set; }
}

public class TagStatisticsResult
{
    public int TotalTags { get; set; }
    public int WithoutCategoryTags { get; set; }
    public int OrphanTags { get; set; }
    public string? MostUsedTagName { get; set; }
    public int MostUsedTagCount { get; set; }
}

public class TagPopularityProjection
{
    public string Name { get; set; } = string.Empty;
    public EventCategory? Category { get; set; }
    public int EventCount { get; set; }
}
