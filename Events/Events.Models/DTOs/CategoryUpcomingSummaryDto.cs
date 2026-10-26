using Events.Models.Enums;

namespace Events.Models.DTOs;

// A category with its number of upcoming events in a period, for the homepage "Browse by interest" tiles
// (GS-REQ-007, 7.8).
public class CategoryUpcomingSummaryDto
{
    public EventCategory CategoryType { get; set; }

    // Category key as used in /Events?category=..., e.g. "FoodDrink".
    public string Name { get; set; } = string.Empty;

    public int UpcomingEventsCount { get; set; }

    // Image set for the category itself; takes priority over the event image when present.
    public string? DefaultImageUrl { get; set; }

    // Image of the nearest upcoming event in the category that has its own image.
    public string? NearestEventImageUrl { get; set; }
}
