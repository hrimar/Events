namespace Events.Web.Models.DTOs;

/// <summary>
/// DTO for toggling favorite event status response.
/// </summary>
public class ToggleFavoriteDto
{
    /// <summary>
    /// The ID of the event that was toggled.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// The new favorite status (true = favorited, false = removed).
    /// </summary>
    public bool IsFavorited { get; set; }

    /// <summary>
    /// A message describing the action that was performed.
    /// </summary>
    public string? Message { get; set; }
}
