using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;

namespace Events.Data.Tests.Repositories.Implementations;

public class PageSeoMetaRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();

    private PageSeoMetaRepository CreatePageSeoMetaRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAllAsync_OrdersById()
    {
        _context.PageSeoMetas.AddRange(
            new PageSeoMeta { Id = 2, PageKey = "events" },
            new PageSeoMeta { Id = 1, PageKey = "home" });
        await _context.SaveChangesAsync();

        var result = await CreatePageSeoMetaRepository().GetAllAsync();

        Assert.Equal(["home", "events"], result.Select(p => p.PageKey));
    }

    [Fact]
    public async Task GetByKeyAsync_KeyNotFound_ReturnsNull()
    {
        Assert.Null(await CreatePageSeoMetaRepository().GetByKeyAsync("unknown"));
    }

    [Fact]
    public async Task UpdateManyAsync_MatchesByPageKeyAndUpdatesFieldsAndTimestamp()
    {
        var existing = new PageSeoMeta { Id = 1, PageKey = "home", TitleBg = "Old" };
        _context.PageSeoMetas.Add(existing);
        await _context.SaveChangesAsync();

        var before = DateTime.UtcNow;
        await CreatePageSeoMetaRepository().UpdateManyAsync([new PageSeoMeta { PageKey = "home", TitleBg = "New" }]);

        var updated = await _context.PageSeoMetas.FindAsync(1);
        Assert.Equal("New", updated!.TitleBg);
        Assert.InRange(updated.UpdatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public async Task UpdateManyAsync_IncomingPageKeyNotInDatabase_IsSilentlyIgnored()
    {
        // The repository matches by PageKey and never inserts - an unknown key from the form
        // (e.g. a stale page removed from SeoPageKeys) is dropped rather than causing an error.
        await CreatePageSeoMetaRepository().UpdateManyAsync([new PageSeoMeta { PageKey = "unknown-page", TitleBg = "X" }]);

        Assert.Empty(_context.PageSeoMetas);
    }
}
