namespace Events.Web.Models;

// Horizontal "Don't miss" card (GS-REQ-006, 6.5; GS-REQ-007, 7.7): poster on the left,
// day and time, title, short description and venue on the right.
public class HomeFeatureCardViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;

    // "Нд 27.09 · 20:00", or "Нд 27.09" when the time is unknown; "Free" is added by the view.
    public string DateText { get; init; } = string.Empty;

    public bool IsFree { get; init; }

    // About 140 characters of the description; null when it is empty or only repeats the title and date.
    public string? Excerpt { get; init; }

    public string Venue { get; init; } = string.Empty;
}
