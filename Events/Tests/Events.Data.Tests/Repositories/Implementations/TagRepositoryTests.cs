using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

public class TagRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();
    private readonly Category _musicCategory = new() { Id = 1, Name = "Music", CategoryType = EventCategory.Music };

    private TagRepository CreateTagRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetByNameAsync_BlankName_ReturnsNullWithoutQuerying()
    {
        Assert.Null(await CreateTagRepository().GetByNameAsync("   "));
    }

    [Fact]
    public async Task GetByNameAsync_MatchesExactTrimmedName()
    {
        _context.Tags.Add(new Tag { Id = 1, Name = "Jazz" });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetByNameAsync("Jazz");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByCategoryAsync_NullCategory_ReturnsTagsWithNoCategory()
    {
        _context.Tags.AddRange(
            new Tag { Id = 1, Name = "Uncategorized", Category = null },
            new Tag { Id = 2, Name = "Live", Category = EventCategory.Music });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetByCategoryAsync(null);

        Assert.Equal("Uncategorized", Assert.Single(result).Name);
    }

    [Fact]
    public async Task AddAsync_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;

        var result = await CreateTagRepository().AddAsync(new Tag { Name = "Jazz" });

        Assert.InRange(result.CreatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public async Task DeleteRangeAsync_EmptyIds_DoesNothing()
    {
        await CreateTagRepository().DeleteRangeAsync([]); // must not throw
    }

    [Fact]
    public async Task DeleteRangeAsync_MixOfExistingAndNonExistingIds_DeletesOnlyExisting()
    {
        _context.Tags.AddRange(new Tag { Id = 1, Name = "Jazz" }, new Tag { Id = 2, Name = "Rock" });
        await _context.SaveChangesAsync();

        await CreateTagRepository().DeleteRangeAsync([1, 999]); // 999 doesn't exist - must not throw

        Assert.Null(await _context.Tags.FindAsync(1));
        Assert.NotNull(await _context.Tags.FindAsync(2));
    }

    // GetPagedAdminTagsAsync

    [Fact]
    public async Task GetPagedAdminTagsAsync_SearchTerm_MatchesViaLikePattern()
    {
        _context.Tags.AddRange(new Tag { Id = 1, Name = "Jazz Night" }, new Tag { Id = 2, Name = "Rock" });
        await _context.SaveChangesAsync();

        var (tags, totalCount) = await CreateTagRepository().GetPagedAdminTagsAsync(
            1, 10, "jazz", null, false, false, "name", "asc", CancellationToken.None);

        Assert.Equal(1, totalCount);
        Assert.Equal("Jazz Night", Assert.Single(tags).Name);
    }

    [Fact]
    public async Task GetPagedAdminTagsAsync_ShowOrphansOnly_ReturnsOnlyTagsWithoutEventTags()
    {
        var taggedTag = new Tag { Id = 1, Name = "Used" };
        var orphanTag = new Tag { Id = 2, Name = "Orphan" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.AddRange(taggedTag, orphanTag);
        _context.Events.Add(new Event
        {
            Id = 1, Name = "Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id
        });
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = taggedTag.Id });
        await _context.SaveChangesAsync();

        var (tags, _) = await CreateTagRepository().GetPagedAdminTagsAsync(
            1, 10, null, null, true, false, "name", "asc", CancellationToken.None);

        Assert.Equal("Orphan", Assert.Single(tags).Name);
    }

    [Fact]
    public async Task GetPagedAdminTagsAsync_UsageCountReflectsEventTagCount()
    {
        var tag = new Tag { Id = 1, Name = "Live" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.Add(tag);
        _context.Events.AddRange(
            new Event { Id = 1, Name = "Show1", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id },
            new Event { Id = 2, Name = "Show2", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id });
        _context.EventTags.AddRange(
            new EventTag { EventId = 1, TagId = tag.Id },
            new EventTag { EventId = 2, TagId = tag.Id });
        await _context.SaveChangesAsync();

        var (tags, _) = await CreateTagRepository().GetPagedAdminTagsAsync(
            1, 10, null, null, false, false, "name", "asc", CancellationToken.None);

        Assert.Equal(2, Assert.Single(tags).UsageCount);
    }

    // GetUsageAggregatesAsync

    [Fact]
    public async Task GetUsageAggregatesAsync_EmptyTagIds_ReturnsEmptyDictionaryWithoutQuerying()
    {
        var result = await CreateTagRepository().GetUsageAggregatesAsync([], CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUsageAggregatesAsync_GroupsByTagAndCountsUsage()
    {
        var tag = new Tag { Id = 1, Name = "Live" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.Add(tag);
        _context.Events.Add(new Event
        {
            Id = 1, Name = "Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id
        });
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = tag.Id });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetUsageAggregatesAsync([tag.Id], CancellationToken.None);

        Assert.Equal(1, result[tag.Id].UsageCount);
        Assert.Contains("Music", result[tag.Id].Categories);
    }

    // GetStatisticsAsync

    [Fact]
    public async Task GetStatisticsAsync_ReportsMostUsedTagByEventTagCount()
    {
        var popularTag = new Tag { Id = 1, Name = "Popular" };
        var unusedTag = new Tag { Id = 2, Name = "Unused" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.AddRange(popularTag, unusedTag);
        _context.Events.Add(new Event
        {
            Id = 1, Name = "Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id
        });
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = popularTag.Id });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetStatisticsAsync(CancellationToken.None);

        Assert.Equal(2, result.TotalTags);
        Assert.Equal(1, result.OrphanTags);
        Assert.Equal("Popular", result.MostUsedTagName);
        Assert.Equal(1, result.MostUsedTagCount);
    }

    // GetOrphanTagIdsAsync

    [Fact]
    public async Task GetOrphanTagIdsAsync_ReturnsOnlyTagsWithoutEventTags()
    {
        var taggedTag = new Tag { Id = 1, Name = "Used" };
        var orphanTag = new Tag { Id = 2, Name = "Orphan" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.AddRange(taggedTag, orphanTag);
        _context.Events.Add(new Event
        {
            Id = 1, Name = "Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id
        });
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = taggedTag.Id });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetOrphanTagIdsAsync();

        Assert.Equal([orphanTag.Id], result);
    }

    // GetPopularTagsAsync

    [Fact]
    public async Task GetPopularTagsAsync_OnlyCountsPublishedEventsOnOrAfterFromDate()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Live" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.Add(tag);
        var pastEvent = new Event
        {
            Id = 1, Name = "Past", Date = new DateTime(2025, 1, 1), City = "Sofia", Location = "NDK",
            CategoryId = _musicCategory.Id, Status = EventStatus.Published
        };
        var draftEvent = new Event
        {
            Id = 2, Name = "Draft", Date = new DateTime(2026, 6, 1), City = "Sofia", Location = "NDK",
            CategoryId = _musicCategory.Id, Status = EventStatus.Draft
        };
        var qualifyingEvent = new Event
        {
            Id = 3, Name = "Qualifying", Date = new DateTime(2026, 6, 1), City = "Sofia", Location = "NDK",
            CategoryId = _musicCategory.Id, Status = EventStatus.Published
        };
        _context.Events.AddRange(pastEvent, draftEvent, qualifyingEvent);
        _context.EventTags.AddRange(
            new EventTag { EventId = pastEvent.Id, TagId = tag.Id },
            new EventTag { EventId = draftEvent.Id, TagId = tag.Id },
            new EventTag { EventId = qualifyingEvent.Id, TagId = tag.Id });
        await _context.SaveChangesAsync();

        // Act
        var result = await CreateTagRepository().GetPopularTagsAsync(fromDate: new DateTime(2026, 1, 1));

        // Assert
        var popular = Assert.Single(result);
        Assert.Equal("Live", popular.Name);
        Assert.Equal(1, popular.EventCount);
    }

    [Fact]
    public async Task GetPopularTagsAsync_TagsWithNoQualifyingEvents_AreExcluded()
    {
        _context.Tags.Add(new Tag { Id = 1, Name = "Unused" });
        await _context.SaveChangesAsync();

        var result = await CreateTagRepository().GetPopularTagsAsync(fromDate: DateTime.UtcNow);

        Assert.Empty(result);
    }
}
