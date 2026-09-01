using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

public class SubCategoryRepositoryTests : IDisposable
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly EventsDbContext _context;
    private readonly Category _musicCategory = new() { Id = 1, Name = "Music", CategoryType = EventCategory.Music };

    public SubCategoryRepositoryTests()
    {
        _context = InMemoryDbContextFactory.Create(_databaseName);
    }

    private SubCategoryRepository CreateSubCategoryRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetByIdAsync_IncludesEvents()
    {
        _context.Categories.Add(_musicCategory);
        var subCategory = new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = _musicCategory.Id };
        _context.SubCategories.Add(subCategory);
        _context.Events.Add(new Event
        {
            Id = 1, Name = "Rock Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK",
            CategoryId = _musicCategory.Id, SubCategoryId = subCategory.Id
        });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Single(result!.Events);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByParentCategoryThenName()
    {
        _context.SubCategories.AddRange(
            new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = 1 },
            new SubCategory { Id = 2, Name = "Football", ParentCategory = EventCategory.Sports, CategoryId = 2 },
            new SubCategory { Id = 3, Name = "Jazz", ParentCategory = EventCategory.Music, CategoryId = 1 });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().GetAllAsync();

        // Music < Sports as enum values, and within Music, Jazz < Rock alphabetically
        Assert.Equal(["Jazz", "Rock", "Football"], result.Select(sc => sc.Name));
    }

    [Fact]
    public async Task GetByCategoryAsync_FiltersByParentCategory()
    {
        _context.SubCategories.AddRange(
            new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = 1 },
            new SubCategory { Id = 2, Name = "Football", ParentCategory = EventCategory.Sports, CategoryId = 2 });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().GetByCategoryAsync(EventCategory.Music);

        Assert.Equal("Rock", Assert.Single(result).Name);
    }

    [Fact]
    public async Task GetByEnumValueAsync_MatchesParentCategoryAndEnumValue()
    {
        _context.SubCategories.Add(new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, EnumValue = 3, CategoryId = 1 });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().GetByEnumValueAsync(EventCategory.Music, 3);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_NameExistsUnderDifferentCategory_ReturnsFalse()
    {
        _context.SubCategories.Add(new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = 1 });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().ExistsByNameAsync(EventCategory.Sports, "Rock");

        Assert.False(result);
    }

    [Fact]
    public async Task GetCountByCategoryAsync_CountsOnlyMatchingCategory()
    {
        _context.SubCategories.AddRange(
            new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = 1 },
            new SubCategory { Id = 2, Name = "Jazz", ParentCategory = EventCategory.Music, CategoryId = 1 },
            new SubCategory { Id = 3, Name = "Football", ParentCategory = EventCategory.Sports, CategoryId = 2 });
        await _context.SaveChangesAsync();

        var result = await CreateSubCategoryRepository().GetCountByCategoryAsync(EventCategory.Music);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetSubCategoriesWithEventsAsync_ExcludesSubCategoriesWithoutEvents_AndOnlyIncludesPublishedEvents()
    {
        // Arrange
        _context.Categories.Add(_musicCategory);
        var withEvents = new SubCategory { Id = 1, Name = "Rock", ParentCategory = EventCategory.Music, CategoryId = _musicCategory.Id };
        var withoutEvents = new SubCategory { Id = 2, Name = "Jazz", ParentCategory = EventCategory.Music, CategoryId = _musicCategory.Id };
        _context.SubCategories.AddRange(withEvents, withoutEvents);
        _context.Events.AddRange(
            new Event
            {
                Id = 1, Name = "Published Rock Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK",
                CategoryId = _musicCategory.Id, SubCategoryId = withEvents.Id, Status = EventStatus.Published
            },
            new Event
            {
                Id = 2, Name = "Draft Rock Show", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK",
                CategoryId = _musicCategory.Id, SubCategoryId = withEvents.Id, Status = EventStatus.Draft
            });
        await _context.SaveChangesAsync();

        // Act - read through a *separate* context instance pointed at the same InMemory database.
        // Reusing _context here would make EF's change-tracker "fixup" attach both the Published
        // and Draft event to Rock.Events regardless of the query's filtered .Include(...) Where
        // clause, since both are already tracked in _context from the seeding above - that would
        // pass even if the repository's filter were broken. A fresh, untracked context exercises
        // the query exactly as it runs in production (a new DbContext per request).
        using var readContext = InMemoryDbContextFactory.Create(_databaseName);
        var result = await new SubCategoryRepository(readContext).GetSubCategoriesWithEventsAsync(EventCategory.Music);

        // Assert - only "Rock" has events at all, and only the Published one is included in its Events collection
        var rock = Assert.Single(result);
        Assert.Equal("Rock", rock.Name);
        Assert.Single(rock.Events);
        Assert.Equal("Published Rock Show", rock.Events.Single().Name);
    }
}
