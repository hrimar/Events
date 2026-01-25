using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    private readonly EventsDbContext _context;

    public TagRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var normalized = name.Trim().ToLower();
        return await _context.Tags
            .AsNoTracking() // Important to avoid tracking issues on updates after that
            .FirstOrDefaultAsync(t => t.Name.ToLower() == normalized);
    }

    public async Task<IEnumerable<Tag>> GetByCategoryAsync(EventCategory? category)
    {
        return await _context.Tags
            .Where(t => t.Category == category)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        tag.CreatedAt = DateTime.UtcNow;

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteRangeAsync(IEnumerable<int> ids)
    {
        var idList = ids?.Distinct().ToList();
        if (idList == null || idList.Count == 0)
        {
            return;
        }

        var tags = await _context.Tags
            .Where(t => idList.Contains(t.Id))
            .ToListAsync();

        if (!tags.Any())
        {
            return;
        }

        _context.Tags.RemoveRange(tags);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<TagAdminProjection> Tags, int TotalCount)> GetPagedAdminTagsAsync(
        int page,
        int pageSize,
        string? searchTerm,
        EventCategory? category,
        bool showOrphansOnly,
        bool showWithoutCategoryOnly,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm}%";
            query = query.Where(t => EF.Functions.Like(t.Name, pattern));
        }

        if (category.HasValue)
        {
            query = query.Where(t => t.Category == category);
        }

        if (showWithoutCategoryOnly)
        {
            query = query.Where(t => t.Category == null);
        }

        if (showOrphansOnly)
        {
            query = query.Where(t => !t.EventTags.Any());
        }

        query = ApplySorting(query, sortBy, sortOrder);

        var totalCount = await query.CountAsync(cancellationToken);

        var tags = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TagAdminProjection
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                CreatedAt = t.CreatedAt,
                UsageCount = t.EventTags.Count
            })
            .ToListAsync(cancellationToken);

        return (tags, totalCount);
    }

    public async Task<Dictionary<int, TagUsageAggregateResult>> GetUsageAggregatesAsync(
        IReadOnlyCollection<int> tagIds,
        CancellationToken cancellationToken)
    {
        if (tagIds.Count == 0)
        {
            return new Dictionary<int, TagUsageAggregateResult>();
        }

        var usageDetails = await _context.EventTags
            .AsNoTracking()
            .Where(et => tagIds.Contains(et.TagId))
            .Select(et => new UsageDetail
            {
                TagId = et.TagId,
                CategoryName = et.Event != null && et.Event.Category != null ? et.Event.Category.Name : null,
                SubCategoryName = et.Event != null && et.Event.SubCategory != null ? et.Event.SubCategory.Name : null
            })
            .ToListAsync(cancellationToken);

        return usageDetails
            .GroupBy(d => d.TagId)
            .ToDictionary(
                g => g.Key,
                g => new TagUsageAggregateResult(
                    g.Count(),
                    BuildOrderedList(g.Select(d => d.CategoryName)),
                    BuildOrderedList(g.Select(d => d.SubCategoryName))));
    }

    public async Task<TagStatisticsResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        var totalTags = await _context.Tags.CountAsync(cancellationToken);
        var withoutCategory = await _context.Tags.CountAsync(t => t.Category == null, cancellationToken);
        var orphanTags = await _context.Tags.CountAsync(t => !t.EventTags.Any(), cancellationToken);

        var topTag = await _context.Tags
            .AsNoTracking()
            .OrderByDescending(t => t.EventTags.Count)
            .Select(t => new { t.Name, Count = t.EventTags.Count })
            .FirstOrDefaultAsync(cancellationToken);

        return new TagStatisticsResult
        {
            TotalTags = totalTags,
            WithoutCategoryTags = withoutCategory,
            OrphanTags = orphanTags,
            MostUsedTagName = topTag?.Name,
            MostUsedTagCount = topTag?.Count ?? 0
        };
    }

    public async Task<List<int>> GetOrphanTagIdsAsync()
    {
        return await _context.Tags
            .Where(t => !t.EventTags.Any())
            .Select(t => t.Id)
            .ToListAsync();
    }

    private static IQueryable<Tag> ApplySorting(IQueryable<Tag> query, string sortBy, string sortOrder)
    {
        var normalizedSort = string.IsNullOrWhiteSpace(sortBy) ? "name" : sortBy.ToLowerInvariant();
        var descending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return normalizedSort switch
        {
            "usage" => descending
                ? query.OrderByDescending(t => t.EventTags.Count).ThenBy(t => t.Name)
                : query.OrderBy(t => t.EventTags.Count).ThenBy(t => t.Name),
            "created" => descending
                ? query.OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Name)
                : query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Name),
            _ => descending
                ? query.OrderByDescending(t => t.Name)
                : query.OrderBy(t => t.Name)
        };
    }

    private static IReadOnlyList<string> BuildOrderedList(IEnumerable<string?> names)
    {
        return names
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!.Trim())
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Key)
            .ToList();
    }

    private sealed class UsageDetail
    {
        public int TagId { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
    }
}