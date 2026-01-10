namespace Events.Web.Models.DTOs;

/// <summary>
/// DTO for checking favorite event status response.
/// </summary>
public class CheckFavoriteDto
{
    /// <summary>
    /// The ID of the event that was checked.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// Whether the event is in the user's favorites.
    /// </summary>
    public bool IsFavorited { get; set; }
}
