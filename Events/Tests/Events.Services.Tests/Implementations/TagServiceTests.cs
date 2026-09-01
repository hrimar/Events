using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Caching;
using Events.Services.Implementations;
using Events.Services.Models.Admin;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventTagRepository> _eventTagRepositoryMock = new();
    private readonly Mock<ILogger<TagService>> _loggerMock = new();
    private readonly Mock<IEventCacheInvalidator> _cacheInvalidatorMock = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public TagServiceTests()
    {
        _cacheInvalidatorMock.Setup(c => c.Token).Returns(CancellationToken.None);
    }

    private TagService CreateTagService() => new(
        _tagRepositoryMock.Object,
        _eventRepositoryMock.Object,
        _eventTagRepositoryMock.Object,
        _loggerMock.Object,
        _cache,
        _cacheInvalidatorMock.Object);

    private static Tag CreateTag(int id = 1, string name = "Jazz") => new() { Id = id, Name = name };

    // GetTagByIdAsync

    [Fact]
    public async Task GetTagByIdAsync_TagExists_ReturnsTag()
    {
        var expectedTag = CreateTag();
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedTag);

        var result = await CreateTagService().GetTagByIdAsync(1);

        Assert.Equal(expectedTag, result);
    }

    [Fact]
    public async Task GetTagByIdAsync_RepositoryThrows_RethrowsSameException()
    {
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateTagService().GetTagByIdAsync(1));
    }

    // GetTagByNameAsync

    [Fact]
    public async Task GetTagByNameAsync_NameNormalizesToEmpty_ReturnsNullWithoutCallingRepository()
    {
        var result = await CreateTagService().GetTagByNameAsync("some (invalid) name");

        Assert.Null(result);
        _tagRepositoryMock.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetTagByNameAsync_ValidName_QueriesRepositoryWithNormalizedName()
    {
        _tagRepositoryMock.Setup(r => r.GetByNameAsync("Jazz")).ReturnsAsync(CreateTag());

        await CreateTagService().GetTagByNameAsync("  Jazz  ");

        _tagRepositoryMock.Verify(r => r.GetByNameAsync("Jazz"), Times.Once);
    }

    // CreateTagAsync

    [Fact]
    public async Task CreateTagAsync_InvalidName_ThrowsArgumentExceptionWithoutInvalidatingCache()
    {
        var tag = CreateTag(name: "bad (name)");

        await Assert.ThrowsAsync<ArgumentException>(() => CreateTagService().CreateTagAsync(tag));
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task CreateTagAsync_NameAlreadyUsedByAnotherTag_ThrowsInvalidOperationException()
    {
        var tag = CreateTag(id: 1, name: "Jazz");
        var existingTag = CreateTag(id: 2, name: "Jazz");
        _tagRepositoryMock.Setup(r => r.GetByNameAsync("Jazz")).ReturnsAsync(existingTag);

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateTagService().CreateTagAsync(tag));
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task CreateTagAsync_ValidTag_AddsAndInvalidatesCache()
    {
        // Arrange
        var tag = CreateTag(name: "  Jazz  ");
        _tagRepositoryMock.Setup(r => r.GetByNameAsync("Jazz")).ReturnsAsync((Tag?)null);
        _tagRepositoryMock.Setup(r => r.AddAsync(tag)).ReturnsAsync(tag);

        // Act
        var result = await CreateTagService().CreateTagAsync(tag);

        // Assert
        Assert.Equal("Jazz", tag.Name);
        Assert.Equal(tag, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // UpdateTagAsync

    [Fact]
    public async Task UpdateTagAsync_NameBelongsToSameTag_UpdatesSuccessfully()
    {
        var tag = CreateTag(id: 1, name: "Jazz");
        _tagRepositoryMock.Setup(r => r.GetByNameAsync("Jazz")).ReturnsAsync(tag);
        _tagRepositoryMock.Setup(r => r.UpdateAsync(tag)).ReturnsAsync(tag);

        var result = await CreateTagService().UpdateTagAsync(tag);

        Assert.Equal(tag, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // DeleteTagAsync

    [Fact]
    public async Task DeleteTagAsync_DeletesAndInvalidatesCache()
    {
        await CreateTagService().DeleteTagAsync(1);

        _tagRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // DeleteTagsAsync

    [Fact]
    public async Task DeleteTagsAsync_EmptyIds_DoesNothing()
    {
        await CreateTagService().DeleteTagsAsync([]);

        _eventTagRepositoryMock.Verify(r => r.RemoveEventTagsByTagIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _tagRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task DeleteTagsAsync_NonEmptyIds_RemovesEventTagsThenDeletesTagsAndInvalidatesCache()
    {
        // Arrange
        var ids = new List<int> { 1, 2, 2 }; // duplicate on purpose

        // Act
        await CreateTagService().DeleteTagsAsync(ids);

        // Assert - duplicates must be collapsed before hitting the repositories
        _eventTagRepositoryMock.Verify(r => r.RemoveEventTagsByTagIdsAsync(
            It.Is<IEnumerable<int>>(x => x.SequenceEqual(new[] { 1, 2 }))), Times.Once);
        _tagRepositoryMock.Verify(r => r.DeleteRangeAsync(
            It.Is<IEnumerable<int>>(x => x.SequenceEqual(new[] { 1, 2 }))), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // DeleteOrphanTagsAsync

    [Fact]
    public async Task DeleteOrphanTagsAsync_NoOrphans_ReturnsZeroWithoutInvalidatingCache()
    {
        _tagRepositoryMock.Setup(r => r.GetOrphanTagIdsAsync()).ReturnsAsync([]);

        var result = await CreateTagService().DeleteOrphanTagsAsync();

        Assert.Equal(0, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task DeleteOrphanTagsAsync_OrphansFound_DeletesAndInvalidatesCache()
    {
        var orphanIds = new List<int> { 5, 6 };
        _tagRepositoryMock.Setup(r => r.GetOrphanTagIdsAsync()).ReturnsAsync(orphanIds);

        var result = await CreateTagService().DeleteOrphanTagsAsync();

        Assert.Equal(2, result);
        _eventTagRepositoryMock.Verify(r => r.RemoveEventTagsByTagIdsAsync(orphanIds), Times.Once);
        _tagRepositoryMock.Verify(r => r.DeleteRangeAsync(orphanIds), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // AddTagToEventAsync

    [Fact]
    public async Task AddTagToEventAsync_AlreadyExists_DoesNotAddAgain()
    {
        _eventTagRepositoryMock.Setup(r => r.EventTagExistsAsync(1, 2)).ReturnsAsync(true);

        await CreateTagService().AddTagToEventAsync(1, 2);

        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(It.IsAny<List<EventTag>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task AddTagToEventAsync_NotYetAssociated_AddsAndInvalidatesCache()
    {
        _eventTagRepositoryMock.Setup(r => r.EventTagExistsAsync(1, 2)).ReturnsAsync(false);

        await CreateTagService().AddTagToEventAsync(1, 2);

        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(
            It.Is<List<EventTag>>(list => list.Count == 1 && list[0].EventId == 1 && list[0].TagId == 2)), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // RemoveTagFromEventAsync

    [Fact]
    public async Task RemoveTagFromEventAsync_TagNotAssociatedWithEvent_DoesNothing()
    {
        _eventTagRepositoryMock.Setup(r => r.GetEventTagsByEventIdAsync(1)).ReturnsAsync([]);

        await CreateTagService().RemoveTagFromEventAsync(1, 2);

        _eventRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task RemoveTagFromEventAsync_TagAssociatedWithEvent_RemovesAndInvalidatesCache()
    {
        // Arrange
        var eventTag = new EventTag { EventId = 1, TagId = 2 };
        _eventTagRepositoryMock.Setup(r => r.GetEventTagsByEventIdAsync(1)).ReturnsAsync([eventTag]);

        var eventEntity = new Event { Id = 1, EventTags = { eventTag } };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(eventEntity);
        _eventRepositoryMock.Setup(r => r.UpdateAsync(eventEntity)).ReturnsAsync(eventEntity);

        // Act
        await CreateTagService().RemoveTagFromEventAsync(1, 2);

        // Assert
        Assert.DoesNotContain(eventTag, eventEntity.EventTags);
        _eventRepositoryMock.Verify(r => r.UpdateAsync(eventEntity), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // GetPopularTagsAsync

    [Fact]
    public async Task GetPopularTagsAsync_CalledTwiceWithSameArguments_HitsRepositoryOnlyOnce()
    {
        var fromDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _tagRepositoryMock
            .Setup(r => r.GetPopularTagsAsync(fromDate, null, null, null, CancellationToken.None))
            .ReturnsAsync([]);
        var sut = CreateTagService();

        await sut.GetPopularTagsAsync(fromDate);
        await sut.GetPopularTagsAsync(fromDate);

        _tagRepositoryMock.Verify(r => r.GetPopularTagsAsync(fromDate, null, null, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetPopularTagsAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _tagRepositoryMock
            .Setup(r => r.GetPopularTagsAsync(It.IsAny<DateTime>(), It.IsAny<string?>(), It.IsAny<EventCategory?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateTagService().GetPopularTagsAsync(DateTime.UtcNow));
    }

    // BulkAddTagsToEventAsync

    [Fact]
    public async Task BulkAddTagsToEventAsync_EmptyTagIds_DoesNothing()
    {
        await CreateTagService().BulkAddTagsToEventAsync(1, []);

        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(It.IsAny<List<EventTag>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task BulkAddTagsToEventAsync_NonEmptyTagIds_AddsAllAndInvalidatesCache()
    {
        await CreateTagService().BulkAddTagsToEventAsync(1, [2, 3]);

        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(
            It.Is<List<EventTag>>(list => list.Count == 2)), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // BulkRemoveTagsFromEventAsync

    [Fact]
    public async Task BulkRemoveTagsFromEventAsync_RemovesAndInvalidatesCache()
    {
        await CreateTagService().BulkRemoveTagsFromEventAsync(1);

        _eventTagRepositoryMock.Verify(r => r.BulkRemoveEventTagsByEventIdAsync(1), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // BulkAssignTagsToMultipleEventsAsync

    [Fact]
    public async Task BulkAssignTagsToMultipleEventsAsync_EmptyEventIds_DoesNothing()
    {
        await CreateTagService().BulkAssignTagsToMultipleEventsAsync([], [1]);

        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(It.IsAny<List<EventTag>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task BulkAssignTagsToMultipleEventsAsync_EventDoesNotExist_SkipsThatEvent()
    {
        // Arrange
        _eventRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(false);

        // Act
        await CreateTagService().BulkAssignTagsToMultipleEventsAsync([1], [10]);

        // Assert
        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(It.IsAny<List<EventTag>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task BulkAssignTagsToMultipleEventsAsync_AlreadyAssignedPairsAreSkipped()
    {
        // Arrange: event 1 exists, tag 10 already assigned to it, tag 11 is not
        _eventRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _eventTagRepositoryMock.Setup(r => r.EventTagExistsAsync(1, 10)).ReturnsAsync(true);
        _eventTagRepositoryMock.Setup(r => r.EventTagExistsAsync(1, 11)).ReturnsAsync(false);

        // Act
        await CreateTagService().BulkAssignTagsToMultipleEventsAsync([1], [10, 11]);

        // Assert - only the not-yet-assigned pair (1, 11) gets added
        _eventTagRepositoryMock.Verify(r => r.BulkAddEventTagsAsync(
            It.Is<List<EventTag>>(list => list.Count == 1 && list[0].EventId == 1 && list[0].TagId == 11)), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }
}
