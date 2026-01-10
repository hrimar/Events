using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for managing user favorite events.
/// Handles database operations for user preferences.
/// </summary>
public interface IUserFavoriteEventRepository
{
    /// <summary>
    /// Adds an event to a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event to favorite.</param>
    /// <returns>The created UserFavoriteEvent or null if it already exists.</returns>
    Task<UserFavoriteEvent?> AddFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Removes an event from a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event to remove.</param>
    /// <returns>True if the favorite was removed, false if it didn't exist.</returns>
    Task<bool> RemoveFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Checks if an event is in a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>True if the event is favorited, false otherwise.</returns>
    Task<bool> IsFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Gets all favorite events for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Collection of user's favorite events with event details.</returns>
    Task<IEnumerable<UserFavoriteEvent>> GetUserFavoritesAsync(string userId);

    /// <summary>
    /// Gets the count of favorite events for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Number of favorite events.</returns>
    Task<int> GetFavoriteCountAsync(string userId);
}
