using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.Repositories.Implementations;

public class UserFavoriteEventRepositoryTests : IDisposable
{
    private readonly EventsDbContext _context = InMemoryDbContextFactory.Create();
    private readonly Category _musicCategory = new() { Id = 1, Name = "Music", CategoryType = EventCategory.Music };

    private UserFavoriteEventRepository CreateUserFavoriteEventRepository() => new(_context);

    // GetUserFavoritesAsync requires the Event (EventId is a required, non-nullable FK), so EF Core
    // treats the .Include(u => u.Event) as a required relationship and generates an INNER JOIN as an
    // optimization - a favorite row pointing at a non-existent Event would silently vanish from the
    // result instead of surfacing with Event == null, so every seeded favorite needs a real Event row.
    private void SeedEvent(int id)
    {
        _context.Categories.Add(_musicCategory);
        _context.Events.Add(new Event
        {
            Id = id, Name = $"Event{id}", Date = DateTime.UtcNow, City = "Sofia", Location = "NDK", CategoryId = _musicCategory.Id
        });
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddFavoriteAsync_NotYetFavorited_AddsAndReturnsFavorite()
    {
        var result = await CreateUserFavoriteEventRepository().AddFavoriteAsync("user1", 1);

        Assert.NotNull(result);
        Assert.Equal("user1", result!.UserId);
        Assert.Equal(1, result.EventId);
    }

    [Fact]
    public async Task AddFavoriteAsync_AlreadyFavorited_ReturnsNullWithoutDuplicating()
    {
        _context.UserFavoriteEvents.Add(new UserFavoriteEvent { UserId = "user1", EventId = 1 });
        await _context.SaveChangesAsync();

        var result = await CreateUserFavoriteEventRepository().AddFavoriteAsync("user1", 1);

        Assert.Null(result);
        Assert.Equal(1, await _context.UserFavoriteEvents.CountAsync(f => f.UserId == "user1" && f.EventId == 1));
    }

    [Fact]
    public async Task RemoveFavoriteAsync_Exists_RemovesAndReturnsTrue()
    {
        _context.UserFavoriteEvents.Add(new UserFavoriteEvent { UserId = "user1", EventId = 1 });
        await _context.SaveChangesAsync();

        var result = await CreateUserFavoriteEventRepository().RemoveFavoriteAsync("user1", 1);

        Assert.True(result);
        Assert.False(await _context.UserFavoriteEvents.AnyAsync(f => f.UserId == "user1" && f.EventId == 1));
    }

    [Fact]
    public async Task RemoveFavoriteAsync_DoesNotExist_ReturnsFalse()
    {
        Assert.False(await CreateUserFavoriteEventRepository().RemoveFavoriteAsync("user1", 1));
    }

    [Fact]
    public async Task GetUserFavoritesAsync_OrdersByAddedAtDescending()
    {
        SeedEvent(1);
        SeedEvent(2);
        _context.UserFavoriteEvents.AddRange(
            new UserFavoriteEvent { UserId = "user1", EventId = 1, AddedAt = new DateTime(2026, 1, 1) },
            new UserFavoriteEvent { UserId = "user1", EventId = 2, AddedAt = new DateTime(2026, 6, 1) });
        await _context.SaveChangesAsync();

        var result = await CreateUserFavoriteEventRepository().GetUserFavoritesAsync("user1");

        Assert.Equal([2, 1], result.Select(f => f.EventId));
    }

    [Fact]
    public async Task GetUserFavoritesAsync_OnlyReturnsFavoritesForRequestedUser()
    {
        SeedEvent(1);
        SeedEvent(2);
        _context.UserFavoriteEvents.AddRange(
            new UserFavoriteEvent { UserId = "user1", EventId = 1 },
            new UserFavoriteEvent { UserId = "user2", EventId = 2 });
        await _context.SaveChangesAsync();

        var result = await CreateUserFavoriteEventRepository().GetUserFavoritesAsync("user1");

        Assert.Equal(1, Assert.Single(result).EventId);
    }

    [Fact]
    public async Task GetFavoriteCountAsync_CountsOnlyRequestedUser()
    {
        _context.UserFavoriteEvents.AddRange(
            new UserFavoriteEvent { UserId = "user1", EventId = 1 },
            new UserFavoriteEvent { UserId = "user1", EventId = 2 },
            new UserFavoriteEvent { UserId = "user2", EventId = 3 });
        await _context.SaveChangesAsync();

        var result = await CreateUserFavoriteEventRepository().GetFavoriteCountAsync("user1");

        Assert.Equal(2, result);
    }
}
