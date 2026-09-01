using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.Repositories.Implementations;

public class EventRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();
    private readonly Category _musicCategory = new() { Id = 1, Name = "Music", CategoryType = EventCategory.Music };
    private readonly Category _sportsCategory = new() { Id = 2, Name = "Sports", CategoryType = EventCategory.Sports };

    private EventRepository CreateEventRepository() => new(_context);

    private static Event CreateEvent(int id, string name, DateTime date, int categoryId,
        EventStatus status = EventStatus.Published, string city = "Sofia", string location = "NDK") => new()
        {
            Id = id,
            Name = name,
            Date = date,
            City = city,
            Location = location,
            CategoryId = categoryId,
            Status = status
        };

    public void Dispose() => _context.Dispose();

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_EventExists_ReturnsEventWithCategoryIncluded()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.Add(CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Music", result!.Category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EventDoesNotExist_ReturnsNull()
    {
        var result = await CreateEventRepository().GetByIdAsync(999);

        Assert.Null(result);
    }

    // ExistsAsync

    [Fact]
    public async Task ExistsAsync_EventExists_ReturnsTrue()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.Add(CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        Assert.True(await CreateEventRepository().ExistsAsync(1));
    }

    [Fact]
    public async Task ExistsAsync_EventDoesNotExist_ReturnsFalse()
    {
        Assert.False(await CreateEventRepository().ExistsAsync(999));
    }

    // AddAsync / UpdateAsync

    [Fact]
    public async Task AddAsync_SetsCreatedAtAndUpdatedAtToUtcNow()
    {
        _context.Categories.Add(_musicCategory);
        await _context.SaveChangesAsync();
        var eventEntity = CreateEvent(0, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id);
        var before = DateTime.UtcNow;

        var result = await CreateEventRepository().AddAsync(eventEntity);

        var after = DateTime.UtcNow;
        Assert.InRange(result.CreatedAt, before, after);
        Assert.InRange(result.UpdatedAt, before, after);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUpdatedAtWithoutChangingCreatedAt()
    {
        _context.Categories.Add(_musicCategory);
        var eventEntity = CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id);
        eventEntity.CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        eventEntity.UpdatedAt = eventEntity.CreatedAt;
        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        eventEntity.Name = "Concert (Rescheduled)";
        var result = await CreateEventRepository().UpdateAsync(eventEntity);

        Assert.Equal(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.CreatedAt);
        Assert.True(result.UpdatedAt > result.CreatedAt);
    }

    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_EventExists_RemovesEvent()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.Add(CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        await CreateEventRepository().DeleteAsync(1);

        Assert.False(await _context.Events.AnyAsync(e => e.Id == 1));
    }

    [Fact]
    public async Task DeleteAsync_EventDoesNotExist_DoesNothing()
    {
        await CreateEventRepository().DeleteAsync(999); // must not throw
    }

    // FindByNameAsync / FindAllByNameAsync

    [Fact]
    public async Task FindByNameAsync_MultipleEventsWithSameName_ReturnsFirstMatch()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Concert", new DateTime(2026, 7, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().FindByNameAsync("Concert");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task FindAllByNameAsync_MultipleEventsWithSameName_ReturnsAllMatches()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Concert", new DateTime(2026, 7, 1), _musicCategory.Id),
            CreateEvent(3, "Other", new DateTime(2026, 8, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().FindAllByNameAsync("Concert");

        Assert.Equal(2, result.Count());
    }

    // GetByDateRangeAsync - inclusive on both ends

    [Fact]
    public async Task GetByDateRangeAsync_BoundaryDates_AreInclusive()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Before", new DateTime(2026, 5, 31), _musicCategory.Id),
            CreateEvent(2, "StartBoundary", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(3, "EndBoundary", new DateTime(2026, 6, 10), _musicCategory.Id),
            CreateEvent(4, "After", new DateTime(2026, 6, 11), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetByDateRangeAsync(new DateTime(2026, 6, 1), new DateTime(2026, 6, 10));

        Assert.Equal(["StartBoundary", "EndBoundary"], result.Select(e => e.Name));
    }

    // GetByCategoryAsync

    [Fact]
    public async Task GetByCategoryAsync_FiltersByCategoryEnumMappedToId()
    {
        _context.Categories.AddRange(_musicCategory, _sportsCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Match", new DateTime(2026, 6, 2), _sportsCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetByCategoryAsync(EventCategory.Music);

        Assert.Equal("Concert", Assert.Single(result).Name);
    }

    // GetByLocationAsync - partial match

    [Fact]
    public async Task GetByLocationAsync_PartialMatch_ReturnsMatchingEvents()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Concert", new DateTime(2026, 6, 1), _musicCategory.Id, location: "NDK, Zala 1"),
            CreateEvent(2, "Match", new DateTime(2026, 6, 2), _musicCategory.Id, location: "Arena Armeec"));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetByLocationAsync("NDK");

        Assert.Equal("Concert", Assert.Single(result).Name);
    }

    // SearchAsync - matches Name, Description, or Location

    [Fact]
    public async Task SearchAsync_MatchesAcrossNameDescriptionAndLocation()
    {
        // Note: the InMemory provider compares strings case-sensitively (ordinal), unlike the
        // case-insensitive SQL Server collation used in production - the search term below is
        // cased to match each field exactly so this test isn't tripped up by that difference.
        _context.Categories.Add(_musicCategory);
        var byName = CreateEvent(1, "Jazz Night", new DateTime(2026, 6, 1), _musicCategory.Id);
        var byDescription = CreateEvent(2, "Concert", new DateTime(2026, 6, 2), _musicCategory.Id);
        byDescription.Description = "A Jazz evening";
        var byLocation = CreateEvent(3, "Show", new DateTime(2026, 6, 3), _musicCategory.Id, location: "Jazz Club");
        var noMatch = CreateEvent(4, "Unrelated", new DateTime(2026, 6, 4), _musicCategory.Id);
        _context.Events.AddRange(byName, byDescription, byLocation, noMatch);
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().SearchAsync("Jazz");

        Assert.Equal(3, result.Count());
    }

    // GetFeaturedEventsAsync - IsFeatured + Published + not in the past

    [Fact]
    public async Task GetFeaturedEventsAsync_OnlyReturnsFeaturedPublishedFutureEvents()
    {
        _context.Categories.Add(_musicCategory);
        var today = DateTime.UtcNow.Date;

        var featuredFuture = CreateEvent(1, "FeaturedFuture", today.AddDays(5), _musicCategory.Id);
        featuredFuture.IsFeatured = true;

        var featuredPast = CreateEvent(2, "FeaturedPast", today.AddDays(-5), _musicCategory.Id);
        featuredPast.IsFeatured = true;

        var notFeatured = CreateEvent(3, "NotFeatured", today.AddDays(5), _musicCategory.Id);

        var featuredDraft = CreateEvent(4, "FeaturedDraft", today.AddDays(5), _musicCategory.Id, status: EventStatus.Draft);
        featuredDraft.IsFeatured = true;

        _context.Events.AddRange(featuredFuture, featuredPast, notFeatured, featuredDraft);
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetFeaturedEventsAsync();

        Assert.Equal("FeaturedFuture", Assert.Single(result).Name);
    }

    // GetTotalEventsCountAsync

    [Fact]
    public async Task GetTotalEventsCountAsync_WithStatusFilter_CountsOnlyMatchingStatus()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Published1", new DateTime(2026, 6, 1), _musicCategory.Id, status: EventStatus.Published),
            CreateEvent(2, "Published2", new DateTime(2026, 6, 2), _musicCategory.Id, status: EventStatus.Published),
            CreateEvent(3, "Draft", new DateTime(2026, 6, 3), _musicCategory.Id, status: EventStatus.Draft));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetTotalEventsCountAsync(EventStatus.Published);

        Assert.Equal(2, result);
    }

    // GetEventsCountInRangeAsync - exclusive upper bound is next calendar day

    [Fact]
    public async Task GetEventsCountInRangeAsync_IncludesEventsAnyTimeDuringToDate()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "EarlyOnToDate", new DateTime(2026, 6, 10, 0, 1, 0), _musicCategory.Id),
            CreateEvent(2, "LateOnToDate", new DateTime(2026, 6, 10, 23, 59, 0), _musicCategory.Id),
            CreateEvent(3, "NextDay", new DateTime(2026, 6, 11, 0, 0, 0), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var result = await CreateEventRepository().GetEventsCountInRangeAsync(new DateTime(2026, 6, 1), new DateTime(2026, 6, 10));

        Assert.Equal(2, result);
    }

    // GetPagedEventsAsync

    [Fact]
    public async Task GetPagedEventsAsync_SubCategoryFilterTakesPriorityOverCategoryFilter()
    {
        // Arrange
        var subCategory = new SubCategory { Id = 1, Name = "Rock", CategoryId = _musicCategory.Id };
        _context.Categories.AddRange(_musicCategory, _sportsCategory);
        _context.SubCategories.Add(subCategory);
        _context.Events.AddRange(
            new Event
            {
                Id = 1,
                Name = "RockShow",
                Date = new DateTime(2026, 6, 1),
                City = "Sofia",
                Location = "NDK",
                CategoryId = _musicCategory.Id,
                SubCategoryId = subCategory.Id
            },
            CreateEvent(2, "OtherMusic", new DateTime(2026, 6, 2), _musicCategory.Id));
        await _context.SaveChangesAsync();

        // Act - both categoryName and subCategoryName given; subCategoryName must win
        var (events, totalCount) = await CreateEventRepository().GetPagedEventsAsync(
            page: 1, pageSize: 10, categoryName: "Music", subCategoryName: "Rock");

        // Assert
        Assert.Equal(1, totalCount);
        Assert.Equal("RockShow", Assert.Single(events).Name);
    }

    [Fact]
    public async Task GetPagedEventsAsync_TotalCountReflectsAllMatches_NotJustCurrentPage()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(Enumerable.Range(1, 5)
            .Select(i => CreateEvent(i, $"Event{i}", new DateTime(2026, 6, i), _musicCategory.Id)));
        await _context.SaveChangesAsync();

        var (events, totalCount) = await CreateEventRepository().GetPagedEventsAsync(page: 1, pageSize: 2);

        Assert.Equal(2, events.Count());
        Assert.Equal(5, totalCount);
    }

    [Fact]
    public async Task GetPagedEventsAsync_SecondPage_SkipsFirstPageResults()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(Enumerable.Range(1, 5)
            .Select(i => CreateEvent(i, $"Event{i}", new DateTime(2026, 6, i), _musicCategory.Id)));
        await _context.SaveChangesAsync();

        var (events, _) = await CreateEventRepository().GetPagedEventsAsync(page: 2, pageSize: 2);

        Assert.Equal(["Event3", "Event4"], events.Select(e => e.Name));
    }

    [Fact]
    public async Task GetPagedEventsAsync_ToDateFilter_IncludesEventsAnyTimeDuringToDate()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "OnToDate", new DateTime(2026, 6, 10, 23, 0, 0), _musicCategory.Id),
            CreateEvent(2, "NextDay", new DateTime(2026, 6, 11, 0, 0, 0), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var (events, _) = await CreateEventRepository().GetPagedEventsAsync(
            page: 1, pageSize: 10, toDate: new DateTime(2026, 6, 10));

        Assert.Equal("OnToDate", Assert.Single(events).Name);
    }

    [Fact]
    public async Task GetPagedEventsAsync_TagNamesFilter_ReturnsOnlyEventsHavingAnyOfTheTags()
    {
        // Arrange
        var tag = new Tag { Id = 1, Name = "Live" };
        _context.Categories.Add(_musicCategory);
        _context.Tags.Add(tag);
        var taggedEvent = CreateEvent(1, "Tagged", new DateTime(2026, 6, 1), _musicCategory.Id);
        var untaggedEvent = CreateEvent(2, "Untagged", new DateTime(2026, 6, 2), _musicCategory.Id);
        _context.Events.AddRange(taggedEvent, untaggedEvent);
        _context.EventTags.Add(new EventTag { EventId = taggedEvent.Id, TagId = tag.Id });
        await _context.SaveChangesAsync();

        // Act
        var (events, _) = await CreateEventRepository().GetPagedEventsAsync(
            page: 1, pageSize: 10, tagNames: ["Live"]);

        // Assert
        Assert.Equal("Tagged", Assert.Single(events).Name);
    }

    [Fact]
    public async Task GetPagedEventsAsync_DefaultSort_OrdersByDateAscending()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Later", new DateTime(2026, 6, 10), _musicCategory.Id),
            CreateEvent(2, "Earlier", new DateTime(2026, 6, 1), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var (events, _) = await CreateEventRepository().GetPagedEventsAsync(page: 1, pageSize: 10);

        Assert.Equal(["Earlier", "Later"], events.Select(e => e.Name));
    }

    [Fact]
    public async Task GetPagedEventsAsync_SortByNameDescending_OrdersAlphabeticallyReversed()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Alpha", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Beta", new DateTime(2026, 6, 2), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var (events, _) = await CreateEventRepository().GetPagedEventsAsync(page: 1, pageSize: 10, sortBy: "name", sortOrder: "desc");

        Assert.Equal(["Beta", "Alpha"], events.Select(e => e.Name));
    }

    // GetFilteredEventsAsync

    [Fact]
    public async Task GetFilteredEventsAsync_SearchMatchesNameDescriptionOrLocation()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Jazz Night", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Other", new DateTime(2026, 6, 2), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var (events, totalCount) = await CreateEventRepository().GetFilteredEventsAsync(new EventListCriteria { Search = "Jazz" });

        Assert.Equal(1, totalCount);
        Assert.Equal("Jazz Night", Assert.Single(events).Name);
    }

    [Fact]
    public async Task GetFilteredEventsAsync_SubCategoryIdTakesPriorityOverCategoryId()
    {
        var subCategory = new SubCategory { Id = 1, Name = "Rock", CategoryId = _musicCategory.Id };
        _context.Categories.AddRange(_musicCategory, _sportsCategory);
        _context.SubCategories.Add(subCategory);
        _context.Events.AddRange(
            new Event
            {
                Id = 1,
                Name = "RockShow",
                Date = new DateTime(2026, 6, 1),
                City = "Sofia",
                Location = "NDK",
                CategoryId = _musicCategory.Id,
                SubCategoryId = subCategory.Id
            },
            CreateEvent(2, "OtherMusic", new DateTime(2026, 6, 2), _musicCategory.Id));
        await _context.SaveChangesAsync();

        var criteria = new EventListCriteria { CategoryId = _musicCategory.Id, SubCategoryId = subCategory.Id };
        var (events, _) = await CreateEventRepository().GetFilteredEventsAsync(criteria);

        Assert.Equal("RockShow", Assert.Single(events).Name);
    }

    // BulkUpdateAsync

    [Fact]
    public async Task BulkUpdateAsync_EmptyCollection_ReturnsZero()
    {
        var result = await CreateEventRepository().BulkUpdateAsync([]);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task BulkUpdateAsync_NonEmptyCollection_UpdatesAllAndReturnsCount()
    {
        _context.Categories.Add(_musicCategory);
        var events = new[]
        {
            CreateEvent(1, "Event1", new DateTime(2026, 6, 1), _musicCategory.Id),
            CreateEvent(2, "Event2", new DateTime(2026, 6, 2), _musicCategory.Id)
        };
        _context.Events.AddRange(events);
        await _context.SaveChangesAsync();

        foreach (var e in events) e.Name += " (Updated)";
        var result = await CreateEventRepository().BulkUpdateAsync(events);

        Assert.Equal(2, result);
        Assert.EndsWith("(Updated)", (await _context.Events.FindAsync(1))!.Name);
    }

    // UpdateCanonicalVenueByLocationAsync

    [Fact]
    public async Task UpdateCanonicalVenueByLocationAsync_UpdatesAllEventsMatchingLocation()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            CreateEvent(1, "Event1", new DateTime(2026, 6, 1), _musicCategory.Id, location: "Old Location Text"),
            CreateEvent(2, "Event2", new DateTime(2026, 6, 2), _musicCategory.Id, location: "Old Location Text"),
            CreateEvent(3, "Event3", new DateTime(2026, 6, 3), _musicCategory.Id, location: "Different Location"));
        await _context.SaveChangesAsync();

        var updatedCount = await CreateEventRepository().UpdateCanonicalVenueByLocationAsync("Old Location Text", canonicalVenueId: 7);

        Assert.Equal(2, updatedCount);
        Assert.Equal(7, (await _context.Events.FindAsync(1))!.CanonicalVenueId);
        Assert.Equal(7, (await _context.Events.FindAsync(2))!.CanonicalVenueId);
        Assert.Null((await _context.Events.FindAsync(3))!.CanonicalVenueId);
    }
}
