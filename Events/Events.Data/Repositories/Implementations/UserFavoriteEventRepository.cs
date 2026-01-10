using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing user favorite events.
/// </summary>
public class UserFavoriteEventRepository : IUserFavoriteEventRepository
{
    private readonly EventsDbContext _context;

    public UserFavoriteEventRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<UserFavoriteEvent?> AddFavoriteAsync(string userId, int eventId)
    {
        // Check if already favorited
        var existing = await _context.UserFavoriteEvents.FirstOrDefaultAsync(u => u.UserId == userId && u.EventId == eventId);

        if (existing != null)
            return null; // Already exists

        var favorite = new UserFavoriteEvent
        {
            UserId = userId,
            EventId = eventId,
            AddedAt = DateTime.UtcNow
        };

        _context.UserFavoriteEvents.Add(favorite);
        await _context.SaveChangesAsync();

        return favorite;
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int eventId)
    {
        var favorite = await _context.UserFavoriteEvents.FirstOrDefaultAsync(u => u.UserId == userId && u.EventId == eventId);

        if (favorite == null)
            return false; // Doesn't exist

        _context.UserFavoriteEvents.Remove(favorite);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsFavoriteAsync(string userId, int eventId)
    {
        return await _context.UserFavoriteEvents.AnyAsync(u => u.UserId == userId && u.EventId == eventId);
    }

    public async Task<IEnumerable<UserFavoriteEvent>> GetUserFavoritesAsync(string userId)
    {
        return await _context.UserFavoriteEvents
            .Where(u => u.UserId == userId)
            .Include(u => u.Event)
            .OrderByDescending(u => u.AddedAt)
            .ToListAsync();
    }

    public async Task<int> GetFavoriteCountAsync(string userId)
    {
        return await _context.UserFavoriteEvents.CountAsync(u => u.UserId == userId);
    }
}
