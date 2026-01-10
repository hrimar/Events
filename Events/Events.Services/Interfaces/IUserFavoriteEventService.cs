using Events.Models.Entities;

namespace Events.Services.Interfaces;

/// <summary>
/// Service interface for managing user favorite events.
/// Provides business logic for favorite event operations.
/// </summary>
public interface IUserFavoriteEventService
{
    /// <summary>
    /// Toggles the favorite status of an event for a user.
    /// If the event is already favorited, it will be removed.
    /// If it's not favorited, it will be added.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>True if the event is now favorited, false if it's been removed.</returns>
    Task<bool> ToggleFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Adds an event to a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>True if successfully added, false if it was already favorited.</returns>
    Task<bool> AddFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Removes an event from a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>True if successfully removed, false if it wasn't favorited.</returns>
    Task<bool> RemoveFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Checks if an event is in a user's favorites.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>True if favorited, false otherwise.</returns>
    Task<bool> IsFavoriteAsync(string userId, int eventId);

    /// <summary>
    /// Gets all favorite events for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Collection of user's favorite events.</returns>
    Task<IEnumerable<UserFavoriteEvent>> GetUserFavoritesAsync(string userId);

    /// <summary>
    /// Gets the count of favorite events for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Number of favorite events.</returns>
    Task<int> GetFavoriteCountAsync(string userId);
}
