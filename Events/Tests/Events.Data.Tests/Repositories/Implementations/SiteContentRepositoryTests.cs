using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.Repositories.Implementations;

public class SiteContentRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();

    private SiteContentRepository CreateSiteContentRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAsync_RowExists_ReturnsExistingRowWithoutCreatingANewOne()
    {
        _context.SiteContents.Add(new SiteContent { HeroTitleBg = "Existing" });
        await _context.SaveChangesAsync();

        var result = await CreateSiteContentRepository().GetAsync();

        Assert.Equal("Existing", result.HeroTitleBg);
        Assert.Equal(1, await _context.SiteContents.CountAsync());
    }

    [Fact]
    public async Task GetAsync_NoRowExists_CreatesDefensiveDefaultRow()
    {
        var result = await CreateSiteContentRepository().GetAsync();

        Assert.NotNull(result);
        Assert.Equal(1, await _context.SiteContents.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        var siteContent = new SiteContent { HeroTitleBg = "Old" };
        _context.SiteContents.Add(siteContent);
        await _context.SaveChangesAsync();

        siteContent.HeroTitleBg = "New";
        await CreateSiteContentRepository().UpdateAsync(siteContent);

        Assert.Equal("New", (await _context.SiteContents.FindAsync(siteContent.Id))!.HeroTitleBg);
    }
}
