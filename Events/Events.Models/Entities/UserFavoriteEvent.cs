using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Events.Models.Entities;

/// <summary>
/// Represents a user's favorite event - stores the relationship between users and events
/// they have marked as favorites/saved for later viewing.
/// </summary>
public class UserFavoriteEvent
{
    /// <summary>
    /// ID of the user who favorited the event (FK to AspNetUsers table)
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the favorited event (FK to Events table)
    /// </summary>
    [Required]
    public int EventId { get; set; }

    /// <summary>
    /// Timestamp when the event was added to favorites
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// Reference to the User who favorited the event
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    /// <summary>
    /// Reference to the favorited Event
    /// </summary>
    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
