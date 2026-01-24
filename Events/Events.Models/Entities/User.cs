using Microsoft.AspNetCore.Identity;

namespace Events.Models.Entities;

/// <summary>
/// Represents an application user extending ASP.NET Core Identity.
/// Contains domain logic for managing user preferences such as favorite events.
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Date and time when the user registered. Stored in UTC.
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for EF Core - collection of user's favorite events.
    /// This is the single source of truth for user's favorite events.
    /// Read-only to ensure all modifications go through domain methods.
    /// </summary>
    public IReadOnlyCollection<UserFavoriteEvent> FavoriteEvents { get; private set; } 
        = new List<UserFavoriteEvent>();

    /// <summary>
    /// Internal setter for EF Core to populate the collection during loading.
    /// </summary>
    internal void SetFavoriteEvents(ICollection<UserFavoriteEvent> events)
    {
        FavoriteEvents = (IReadOnlyCollection<UserFavoriteEvent>)events;
    }

    /// <summary>
    /// Adds an event to the user's favorites if not already saved.
    /// </summary>
    /// <param name="eventId">The ID of the event to favorite.</param>
    public void AddFavoriteEvent(int eventId)
    {
        if (FavoriteEvents.Any(e => e.EventId == eventId))
            return; // Already saved

        ((ICollection<UserFavoriteEvent>)FavoriteEvents).Add(new UserFavoriteEvent
        {
            UserId = Id,
            EventId = eventId,
            AddedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Removes an event from the user's favorites.
    /// </summary>
    /// <param name="eventId">The ID of the event to remove from favorites.</param>
    public void RemoveFavoriteEvent(int eventId)
    {
        var favorite = FavoriteEvents.FirstOrDefault(e => e.EventId == eventId);
        if (favorite != null)
            ((ICollection<UserFavoriteEvent>)FavoriteEvents).Remove(favorite);
    }

    /// <summary>
    /// Checks if an event is in the user's favorites.
    /// </summary>
    /// <param name="eventId">The ID of the event to check.</param>
    /// <returns>True if the event is favorited, false otherwise.</returns>
    public bool IsFavorite(int eventId) => FavoriteEvents.Any(e => e.EventId == eventId);

    /// <summary>
    /// Clears all favorite events for the user.
    /// </summary>
    public void ClearAllFavorites()
    {
        ((ICollection<UserFavoriteEvent>)FavoriteEvents).Clear();
    }
}
