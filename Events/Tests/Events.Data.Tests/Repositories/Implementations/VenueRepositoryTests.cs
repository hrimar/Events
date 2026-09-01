using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

public class VenueRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();
    private readonly Category _musicCategory = new() { Id = 1, Name = "Music", CategoryType = EventCategory.Music };

    private VenueRepository CreateVenueRepository() => new(_context);

    private static CanonicalVenue CreateVenue(int id, string name, string slug) =>
        new() { Id = id, Name = name, NameEn = name, Slug = slug, City = "Sofia" };

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetByIdAsync_IncludesAliases()
    {
        var venue = CreateVenue(1, "NDK", "ndk");
        _context.CanonicalVenues.Add(venue);
        _context.VenueAliases.Add(new VenueAlias { CanonicalVenueId = venue.Id, AliasString = "National Palace", NormalizedString = "national palace" });
        await _context.SaveChangesAsync();

        var result = await CreateVenueRepository().GetByIdAsync(1);

        Assert.Single(result!.Aliases);
    }

    [Fact]
    public async Task SlugExistsAsync_SlugTaken_ReturnsTrue()
    {
        _context.CanonicalVenues.Add(CreateVenue(1, "NDK", "ndk"));
        await _context.SaveChangesAsync();

        Assert.True(await CreateVenueRepository().SlugExistsAsync("ndk"));
        Assert.False(await CreateVenueRepository().SlugExistsAsync("arena"));
    }

    [Fact]
    public async Task FindByNormalizedAliasAsync_ReturnsOwningVenue()
    {
        var venue = CreateVenue(1, "NDK", "ndk");
        _context.CanonicalVenues.Add(venue);
        _context.VenueAliases.Add(new VenueAlias { CanonicalVenueId = venue.Id, AliasString = "NDK", NormalizedString = "ndk" });
        await _context.SaveChangesAsync();

        var result = await CreateVenueRepository().FindByNormalizedAliasAsync("ndk");

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task FindByNormalizedAliasAsync_NoMatch_ReturnsNull()
    {
        Assert.Null(await CreateVenueRepository().FindByNormalizedAliasAsync("unknown"));
    }

    // DeleteAsync / DeleteAliasAsync - unlike most other repositories in this project (which
    // silently no-op when the row doesn't exist), these two throw KeyNotFoundException instead.

    [Fact]
    public async Task DeleteAsync_VenueDoesNotExist_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateVenueRepository().DeleteAsync(999));
    }

    [Fact]
    public async Task DeleteAsync_VenueExists_RemovesVenue()
    {
        _context.CanonicalVenues.Add(CreateVenue(1, "NDK", "ndk"));
        await _context.SaveChangesAsync();

        await CreateVenueRepository().DeleteAsync(1);

        Assert.Null(await _context.CanonicalVenues.FindAsync(1));
    }

    [Fact]
    public async Task DeleteAliasAsync_AliasDoesNotExist_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateVenueRepository().DeleteAliasAsync(999));
    }

    // GetAllWithStatsAsync

    [Fact]
    public async Task GetAllWithStatsAsync_ComputesAliasAndEventCounts()
    {
        // Arrange
        var venue = CreateVenue(1, "NDK", "ndk");
        _context.Categories.Add(_musicCategory);
        _context.CanonicalVenues.Add(venue);
        _context.VenueAliases.Add(new VenueAlias { CanonicalVenueId = venue.Id, AliasString = "Alias1", NormalizedString = "alias1" });
        var now = DateTime.UtcNow;
        _context.Events.AddRange(
            new Event { Id = 1, Name = "Upcoming", Date = now.AddDays(5), City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id, CanonicalVenueId = venue.Id, Status = EventStatus.Published },
            new Event { Id = 2, Name = "Past", Date = now.AddDays(-5), City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id, CanonicalVenueId = venue.Id, Status = EventStatus.Published });
        await _context.SaveChangesAsync();

        // Act
        var result = await CreateVenueRepository().GetAllWithStatsAsync();

        // Assert
        var stats = Assert.Single(result);
        Assert.Equal(1, stats.AliasCount);
        Assert.Equal(2, stats.TotalEventsCount);
        Assert.Equal(1, stats.UpcomingEventsCount);
    }

    // GetUnmappedLocationsAsync

    [Fact]
    public async Task GetUnmappedLocationsAsync_GroupsByLocationAndOrdersByCountDescending()
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.AddRange(
            new Event { Id = 1, Name = "E1", Date = DateTime.UtcNow, City = "Sofia", Location = "Unmapped A", CategoryId = _musicCategory.Id },
            new Event { Id = 2, Name = "E2", Date = DateTime.UtcNow, City = "Sofia", Location = "Unmapped A", CategoryId = _musicCategory.Id },
            new Event { Id = 3, Name = "E3", Date = DateTime.UtcNow, City = "Sofia", Location = "Unmapped B", CategoryId = _musicCategory.Id },
            new Event { Id = 4, Name = "E4", Date = DateTime.UtcNow, City = "Sofia", Location = "Mapped", CategoryId = _musicCategory.Id, CanonicalVenueId = 1 });
        await _context.SaveChangesAsync();

        var result = (await CreateVenueRepository().GetUnmappedLocationsAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Unmapped A", result[0].Location);
        Assert.Equal(2, result[0].EventCount);
    }

    // GetUpcomingEventsByVenueAsync

    [Fact]
    public async Task GetUpcomingEventsByVenueAsync_OnlyReturnsPublishedFutureEventsForThatVenue()
    {
        _context.Categories.Add(_musicCategory);
        var now = DateTime.UtcNow;
        _context.Events.AddRange(
            new Event { Id = 1, Name = "MatchingUpcoming", Date = now.AddDays(5), City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id, CanonicalVenueId = 1, Status = EventStatus.Published },
            new Event { Id = 2, Name = "MatchingPast", Date = now.AddDays(-5), City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id, CanonicalVenueId = 1, Status = EventStatus.Published },
            new Event { Id = 3, Name = "OtherVenue", Date = now.AddDays(5), City = "Sofia", Location = "Arena", CategoryId = _musicCategory.Id, CanonicalVenueId = 2, Status = EventStatus.Published });
        await _context.SaveChangesAsync();

        var result = await CreateVenueRepository().GetUpcomingEventsByVenueAsync(1);

        Assert.Equal("MatchingUpcoming", Assert.Single(result).Name);
    }
}
