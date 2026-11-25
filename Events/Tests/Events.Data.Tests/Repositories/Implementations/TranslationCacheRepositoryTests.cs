using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;

namespace Events.Data.Tests.Repositories.Implementations;

public class TranslationCacheRepositoryTests : IDisposable
{
    private const string Hash = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();

    private TranslationCacheRepository CreateRepository() => new(_context);

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAsync_UnknownHash_ReturnsNull()
    {
        Assert.Null(await CreateRepository().GetAsync(Hash));
    }

    [Fact]
    public async Task AddAsync_ThenGetAsync_ReturnsTheTranslationAndSetsCreatedAt()
    {
        var before = DateTime.UtcNow;

        await CreateRepository().AddAsync(new TranslationCacheEntry { Hash = Hash, TranslatedText = "Swan Lake", Model = "m" });

        var entry = await CreateRepository().GetAsync(Hash);
        Assert.NotNull(entry);
        Assert.Equal("Swan Lake", entry.TranslatedText);
        Assert.True(entry.CreatedAt >= before);
    }

    [Fact]
    public async Task AddAsync_ExistingHash_KeepsTheFirstEntry()
    {
        var repository = CreateRepository();
        await repository.AddAsync(new TranslationCacheEntry { Hash = Hash, TranslatedText = "First", Model = "m" });

        await repository.AddAsync(new TranslationCacheEntry { Hash = Hash, TranslatedText = "Second", Model = "m" });

        Assert.Equal("First", (await repository.GetAsync(Hash))!.TranslatedText);
        Assert.Single(_context.TranslationCacheEntries);
    }
}
