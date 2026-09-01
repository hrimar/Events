using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Services.Caching;
using Events.Services.Implementations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<ILogger<EventService>> _loggerMock = new();
    private readonly Mock<IEventCacheInvalidator> _cacheInvalidatorMock = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public EventServiceTests()
    {
        _cacheInvalidatorMock.Setup(c => c.Token).Returns(CancellationToken.None);
    }

    private EventService CreateEventService() =>
        new(_eventRepositoryMock.Object, _loggerMock.Object, _cache, _cacheInvalidatorMock.Object);

    private static readonly DateTime FixedEventDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static Event CreateEvent(int id = 1, string name = "Test Event") =>
        new() { Id = id, Name = name, City = "Sofia", Location = "Test Venue", Date = FixedEventDate };

    // GetEventByIdAsync

    [Fact]
    public async Task GetEventByIdAsync_EventExists_ReturnsEvent()
    {
        var expectedEvent = CreateEvent();
        _eventRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvent);

        var result = await CreateEventService().GetEventByIdAsync(1);

        Assert.Equal(expectedEvent, result);
    }

    [Fact]
    public async Task GetEventByIdAsync_EventDoesNotExist_ReturnsNull()
    {
        _eventRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);

        var result = await CreateEventService().GetEventByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetEventByIdAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetEventByIdAsync(1));
    }

    // GetAllEventsAsync

    [Fact]
    public async Task GetAllEventsAsync_RepositoryReturnsEvents_ReturnsSameEvents()
    {
        var events = new[] { CreateEvent(1), CreateEvent(2) };
        _eventRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

        var result = await CreateEventService().GetAllEventsAsync();

        Assert.Equal(events, result);
    }

    [Fact]
    public async Task GetAllEventsAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetAllEventsAsync());
    }

    // FindEventByNameAsync

    [Fact]
    public async Task FindEventByNameAsync_RepositoryThrows_ReturnsNullInsteadOfThrowing()
    {
        _eventRepositoryMock
            .Setup(r => r.FindByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateEventService().FindEventByNameAsync("Concert");

        Assert.Null(result);
    }

    // FindEventsByNameAsync

    [Fact]
    public async Task FindEventsByNameAsync_RepositoryThrows_ReturnsEmptyInsteadOfThrowing()
    {
        _eventRepositoryMock
            .Setup(r => r.FindAllByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateEventService().FindEventsByNameAsync("Concert");

        Assert.Empty(result);
    }

    // GetPagedEventsAsync

    [Fact]
    public async Task GetPagedEventsAsync_NegativePage_NormalizesToOne()
    {
        var events = new[] { CreateEvent() };
        _eventRepositoryMock
            .Setup(r => r.GetPagedEventsAsync(1, 12, null, null, null, null, null, null, "asc", null, null, CancellationToken.None))
            .ReturnsAsync((events.AsEnumerable(), 1));

        var (result, totalCount) = await CreateEventService().GetPagedEventsAsync(page: -5, pageSize: 0);

        Assert.Equal(events, result);
        Assert.Equal(1, totalCount);
    }

    [Fact]
    public async Task GetPagedEventsAsync_PageSizeExceedsLimit_ClampsTo50000()
    {
        _eventRepositoryMock
            .Setup(r => r.GetPagedEventsAsync(1, 50000, null, null, null, null, null, null, "asc", null, null, CancellationToken.None))
            .ReturnsAsync((Enumerable.Empty<Event>(), 0));

        await CreateEventService().GetPagedEventsAsync(page: 1, pageSize: 100_000);

        _eventRepositoryMock.Verify(r => r.GetPagedEventsAsync(
            1, 50000, null, null, null, null, null, null, "asc", null, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_CalledTwiceWithSameArguments_HitsRepositoryOnlyOnce()
    {
        _eventRepositoryMock
            .Setup(r => r.GetPagedEventsAsync(1, 12, null, null, null, null, null, null, "asc", null, null, CancellationToken.None))
            .ReturnsAsync((Enumerable.Empty<Event>(), 0));
        var sut = CreateEventService();

        await sut.GetPagedEventsAsync(page: 1, pageSize: 12);
        await sut.GetPagedEventsAsync(page: 1, pageSize: 12);

        _eventRepositoryMock.Verify(r => r.GetPagedEventsAsync(
            1, 12, null, null, null, null, null, null, "asc", null, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock
            .Setup(r => r.GetPagedEventsAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<EventStatus?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetPagedEventsAsync(page: 1, pageSize: 12));
    }

    // GetFilteredEventsAsync

    [Fact]
    public async Task GetFilteredEventsAsync_RepositoryReturnsEvents_ReturnsSameEvents()
    {
        var criteria = new EventListCriteria();
        var events = new[] { CreateEvent() };
        _eventRepositoryMock.Setup(r => r.GetFilteredEventsAsync(criteria)).ReturnsAsync((events.AsEnumerable(), 1));

        var (result, totalCount) = await CreateEventService().GetFilteredEventsAsync(criteria);

        Assert.Equal(events, result);
        Assert.Equal(1, totalCount);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_RepositoryThrows_WrapsInApplicationException()
    {
        var criteria = new EventListCriteria();
        _eventRepositoryMock.Setup(r => r.GetFilteredEventsAsync(criteria)).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetFilteredEventsAsync(criteria));
    }

    // GetFeaturedEventsAsync

    [Fact]
    public async Task GetFeaturedEventsAsync_CountExceedsLimit_ClampsTo50()
    {
        _eventRepositoryMock.Setup(r => r.GetFeaturedEventsAsync(50)).ReturnsAsync(Enumerable.Empty<Event>());

        await CreateEventService().GetFeaturedEventsAsync(count: 100);

        _eventRepositoryMock.Verify(r => r.GetFeaturedEventsAsync(50), Times.Once);
    }

    // GetUpcomingEventsAsync

    [Fact]
    public async Task GetUpcomingEventsAsync_CountExceedsLimit_ClampsTo100()
    {
        _eventRepositoryMock.Setup(r => r.GetUpcomingEventsAsync(100)).ReturnsAsync(Enumerable.Empty<Event>());

        await CreateEventService().GetUpcomingEventsAsync(count: 500);

        _eventRepositoryMock.Verify(r => r.GetUpcomingEventsAsync(100), Times.Once);
    }

    // SearchEventsAsync

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchEventsAsync_BlankSearchTerm_ReturnsEmptyWithoutCallingRepository(string? searchTerm)
    {
        var result = await CreateEventService().SearchEventsAsync(searchTerm!);

        Assert.Empty(result);
        _eventRepositoryMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchEventsAsync_TermWithSurroundingWhitespace_TrimsBeforeCallingRepository()
    {
        _eventRepositoryMock
            .Setup(r => r.SearchAsync("concert", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Event>());

        await CreateEventService().SearchEventsAsync("  concert  ");

        _eventRepositoryMock.Verify(r => r.SearchAsync("concert", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchEventsAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock
            .Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().SearchEventsAsync("concert"));
    }

    // GetEventsByCategoryAsync

    [Fact]
    public async Task GetEventsByCategoryAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock
            .Setup(r => r.GetByCategoryAsync(It.IsAny<EventCategory>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetEventsByCategoryAsync(EventCategory.Music));
    }

    // GetEventsByDateRangeAsync

    [Fact]
    public async Task GetEventsByDateRangeAsync_StartDateAfterEndDate_ThrowsApplicationExceptionWrappingArgumentException()
    {
        var start = new DateTime(2026, 1, 10);
        var end = new DateTime(2026, 1, 1);

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetEventsByDateRangeAsync(start, end));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Fact]
    public async Task GetEventsByDateRangeAsync_ValidRange_ReturnsRepositoryResult()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 1, 10);
        var events = new[] { CreateEvent() };
        _eventRepositoryMock.Setup(r => r.GetByDateRangeAsync(start, end)).ReturnsAsync(events);

        var result = await CreateEventService().GetEventsByDateRangeAsync(start, end);

        Assert.Equal(events, result);
    }

    // CreateEventAsync

    [Fact]
    public async Task CreateEventAsync_BlankName_ThrowsApplicationExceptionWrappingArgumentException()
    {
        var eventEntity = CreateEvent(name: "   ");

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().CreateEventAsync(eventEntity));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Fact]
    public async Task CreateEventAsync_ValidEvent_AddsAndInvalidatesCache()
    {
        var eventEntity = CreateEvent();
        _eventRepositoryMock.Setup(r => r.AddAsync(eventEntity)).ReturnsAsync(eventEntity);

        var result = await CreateEventService().CreateEventAsync(eventEntity);

        Assert.Equal(eventEntity, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    [Fact]
    public async Task CreateEventAsync_RepositoryThrows_WrapsInApplicationExceptionAndDoesNotInvalidateCache()
    {
        var eventEntity = CreateEvent();
        _eventRepositoryMock.Setup(r => r.AddAsync(eventEntity)).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().CreateEventAsync(eventEntity));
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    // UpdateEventAsync

    [Fact]
    public async Task UpdateEventAsync_EventDoesNotExist_ThrowsApplicationExceptionWrappingInvalidOperationException()
    {
        var eventEntity = CreateEvent();
        _eventRepositoryMock.Setup(r => r.ExistsAsync(eventEntity.Id)).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().UpdateEventAsync(eventEntity));
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_EventExists_UpdatesAndInvalidatesCache()
    {
        var eventEntity = CreateEvent();
        _eventRepositoryMock.Setup(r => r.ExistsAsync(eventEntity.Id)).ReturnsAsync(true);
        _eventRepositoryMock.Setup(r => r.UpdateAsync(eventEntity)).ReturnsAsync(eventEntity);

        var result = await CreateEventService().UpdateEventAsync(eventEntity);

        Assert.Equal(eventEntity, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // DeleteEventAsync

    [Fact]
    public async Task DeleteEventAsync_EventDoesNotExist_ThrowsApplicationExceptionWrappingInvalidOperationException()
    {
        _eventRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().DeleteEventAsync(1));
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task DeleteEventAsync_EventExists_DeletesAndInvalidatesCache()
    {
        _eventRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        await CreateEventService().DeleteEventAsync(1);

        _eventRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    // GetTotalEventsCountAsync

    [Fact]
    public async Task GetTotalEventsCountAsync_RepositoryThrows_WrapsInApplicationException()
    {
        _eventRepositoryMock.Setup(r => r.GetTotalEventsCountAsync(It.IsAny<EventStatus?>())).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<ApplicationException>(() => CreateEventService().GetTotalEventsCountAsync());
    }

    // EventExistsAsync

    [Fact]
    public async Task EventExistsAsync_RepositoryThrows_ReturnsFalseInsteadOfThrowing()
    {
        _eventRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateEventService().EventExistsAsync(1);

        Assert.False(result);
    }

    // BulkUpdateEventsAsync

    [Fact]
    public async Task BulkUpdateEventsAsync_EmptyCollection_ReturnsZeroWithoutCallingRepository()
    {
        var result = await CreateEventService().BulkUpdateEventsAsync([]);

        Assert.Equal(0, result);
        _eventRepositoryMock.Verify(r => r.BulkUpdateAsync(It.IsAny<IEnumerable<Event>>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task BulkUpdateEventsAsync_NonEmptyCollection_UpdatesAndInvalidatesCache()
    {
        var events = new[] { CreateEvent(1), CreateEvent(2) };
        _eventRepositoryMock.Setup(r => r.BulkUpdateAsync(events)).ReturnsAsync(2);

        var result = await CreateEventService().BulkUpdateEventsAsync(events);

        Assert.Equal(2, result);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }
}
