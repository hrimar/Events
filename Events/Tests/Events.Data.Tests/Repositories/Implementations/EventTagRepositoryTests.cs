using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.Repositories.Implementations;

public class EventTagRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();

    private EventTagRepository CreateEventTagRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    // BulkAddEventTagsAsync

    [Fact]
    public async Task BulkAddEventTagsAsync_EmptyList_DoesNothing()
    {
        await CreateEventTagRepository().BulkAddEventTagsAsync([]); // must not throw
    }

    [Fact]
    public async Task BulkAddEventTagsAsync_DuplicatesWithinIncomingBatch_AreCollapsedToOneRow()
    {
        await CreateEventTagRepository().BulkAddEventTagsAsync(
        [
            new EventTag { EventId = 1, TagId = 10 },
            new EventTag { EventId = 1, TagId = 10 } // exact duplicate pair in the same call
        ]);

        Assert.Equal(1, await _context.EventTags.CountAsync());
    }

    [Fact]
    public async Task BulkAddEventTagsAsync_PairAlreadyInDatabase_IsSkippedWithoutError()
    {
        // Arrange - (1, 10) already exists; the incoming batch repeats it plus one genuinely new pair
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = 10 });
        await _context.SaveChangesAsync();

        // Act
        await CreateEventTagRepository().BulkAddEventTagsAsync(
        [
            new EventTag { EventId = 1, TagId = 10 },
            new EventTag { EventId = 1, TagId = 11 }
        ]);

        // Assert - only the new pair was added, the existing one wasn't duplicated
        Assert.Equal(2, await _context.EventTags.CountAsync());
        Assert.True(await _context.EventTags.AnyAsync(et => et.EventId == 1 && et.TagId == 11));
    }

    // BulkRemoveEventTagsByEventIdAsync

    [Fact]
    public async Task BulkRemoveEventTagsByEventIdAsync_RemovesOnlyTagsForThatEvent()
    {
        _context.EventTags.AddRange(
            new EventTag { EventId = 1, TagId = 10 },
            new EventTag { EventId = 1, TagId = 11 },
            new EventTag { EventId = 2, TagId = 10 });
        await _context.SaveChangesAsync();

        await CreateEventTagRepository().BulkRemoveEventTagsByEventIdAsync(1);

        Assert.Equal(0, await _context.EventTags.CountAsync(et => et.EventId == 1));
        Assert.Equal(1, await _context.EventTags.CountAsync(et => et.EventId == 2));
    }

    // GetEventTagsBulkAsync

    [Fact]
    public async Task GetEventTagsBulkAsync_GroupsTagNamesByEventId()
    {
        var tag1 = new Tag { Id = 10, Name = "Live" };
        var tag2 = new Tag { Id = 11, Name = "Rock" };
        _context.Tags.AddRange(tag1, tag2);
        _context.EventTags.AddRange(
            new EventTag { EventId = 1, TagId = tag1.Id },
            new EventTag { EventId = 1, TagId = tag2.Id },
            new EventTag { EventId = 2, TagId = tag1.Id });
        await _context.SaveChangesAsync();

        var result = await CreateEventTagRepository().GetEventTagsBulkAsync([1, 2]);

        Assert.Equal(["Live", "Rock"], result[1].OrderBy(n => n));
        Assert.Equal(["Live"], result[2]);
    }

    [Fact]
    public async Task GetEventTagsBulkAsync_EmptyEventIds_ReturnsEmptyDictionaryWithoutQuerying()
    {
        var result = await CreateEventTagRepository().GetEventTagsBulkAsync([]);

        Assert.Empty(result);
    }

    // EventTagExistsAsync

    [Fact]
    public async Task EventTagExistsAsync_PairExists_ReturnsTrue()
    {
        _context.EventTags.Add(new EventTag { EventId = 1, TagId = 10 });
        await _context.SaveChangesAsync();

        Assert.True(await CreateEventTagRepository().EventTagExistsAsync(1, 10));
        Assert.False(await CreateEventTagRepository().EventTagExistsAsync(1, 99));
    }

    // RemoveEventTagsByTagIdsAsync

    [Fact]
    public async Task RemoveEventTagsByTagIdsAsync_EmptyIds_DoesNothing()
    {
        await CreateEventTagRepository().RemoveEventTagsByTagIdsAsync([]); // must not throw
    }

    [Fact]
    public async Task RemoveEventTagsByTagIdsAsync_RemovesAllRowsForGivenTagIds()
    {
        _context.EventTags.AddRange(
            new EventTag { EventId = 1, TagId = 10 },
            new EventTag { EventId = 2, TagId = 10 },
            new EventTag { EventId = 3, TagId = 11 });
        await _context.SaveChangesAsync();

        await CreateEventTagRepository().RemoveEventTagsByTagIdsAsync([10]);

        Assert.Equal(0, await _context.EventTags.CountAsync(et => et.TagId == 10));
        Assert.Equal(1, await _context.EventTags.CountAsync(et => et.TagId == 11));
    }
}
