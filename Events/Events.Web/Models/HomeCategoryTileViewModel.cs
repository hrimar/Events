using Events.Models.Enums;

namespace Events.Web.Models;

// Category tile of "Browse by interest" (GS-REQ-006, 6.5; GS-REQ-007, 7.8). The name is localized in the view,
// so the cached data stays the same for both languages.
public class HomeCategoryTileViewModel
{
    public EventCategory Category { get; init; }

    // Value for /Events?category=..., e.g. "Theatre".
    public string CategoryKey { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = string.Empty;
}
