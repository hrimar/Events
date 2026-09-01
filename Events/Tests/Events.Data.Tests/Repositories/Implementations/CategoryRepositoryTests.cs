using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

public class CategoryRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();

    private CategoryRepository CreateCategoryRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetByIdAsync_CategoryDoesNotExist_ReturnsNull()
    {
        Assert.Null(await CreateCategoryRepository().GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_OrdersByName()
    {
        _context.Categories.AddRange(
            new Category { Id = 1, Name = "Sports", CategoryType = EventCategory.Sports },
            new Category { Id = 2, Name = "Art", CategoryType = EventCategory.Art });
        await _context.SaveChangesAsync();

        var result = await CreateCategoryRepository().GetAllAsync();

        Assert.Equal(["Art", "Sports"], result.Select(c => c.Name));
    }

    [Fact]
    public async Task GetByTypeAsync_ReturnsMatchingCategory()
    {
        _context.Categories.Add(new Category { Id = 1, Name = "Music", CategoryType = EventCategory.Music });
        await _context.SaveChangesAsync();

        var result = await CreateCategoryRepository().GetByTypeAsync(EventCategory.Music);

        Assert.NotNull(result);
        Assert.Equal("Music", result!.Name);
    }

    [Fact]
    public async Task AddAsync_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;

        var result = await CreateCategoryRepository().AddAsync(new Category { Name = "Music", CategoryType = EventCategory.Music });

        Assert.InRange(result.CreatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public async Task DeleteAsync_CategoryExists_RemovesCategory()
    {
        _context.Categories.Add(new Category { Id = 1, Name = "Music", CategoryType = EventCategory.Music });
        await _context.SaveChangesAsync();

        await CreateCategoryRepository().DeleteAsync(1);

        Assert.Null(await _context.Categories.FindAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_CategoryDoesNotExist_DoesNothing()
    {
        await CreateCategoryRepository().DeleteAsync(999); // must not throw
    }
}
