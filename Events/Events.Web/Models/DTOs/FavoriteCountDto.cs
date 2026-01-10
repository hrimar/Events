namespace Events.Web.Models.DTOs;

/// <summary>
/// DTO for favorite count response.
/// </summary>
public class FavoriteCountDto
{
    /// <summary>
    /// The number of favorite events for the user.
    /// </summary>
    public int Count { get; set; }
}
