namespace Events.Web.Models;

/// <summary>
/// View model for a reusable events section (Featured, Saved, Recommended, etc.)
/// Used by the _EventsSection partial view to render any list of events
/// with a consistent card layout on the home page.
/// </summary>
public class EventsSectionViewModel
{
    /// <summary>Localized section heading — passed already translated from the controller.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Events to display in this section.</summary>
    public List<EventViewModel> Events { get; set; } = new();

    /// <summary>Maximum number of cards to show.</summary>
    public int MaxItems { get; set; } = 6;

    /// <summary>
    /// Optional URL for the "View all" button. Null means the button is not rendered.
    /// </summary>
    public string? ViewAllUrl { get; set; }

    /// <summary>Localized text for the "View all" button.</summary>
    public string ViewAllText { get; set; } = string.Empty;

    /// <summary>Localized categories — needed to display translated category badges on cards.</summary>
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();

    // ---------------------------------------------------------------------------
    // Factory methods — keep the controller free of mapping boilerplate
    // ---------------------------------------------------------------------------

    public static EventsSectionViewModel CreateFeaturedSection(List<EventViewModel> events, List<CategoryDisplayItem> categories,
        string title, string viewAllText, string viewAllUrl) => new()
        {
            Title = title,
            Events = events,
            MaxItems = 18,
            ViewAllUrl = viewAllUrl,
            ViewAllText = viewAllText,
            LocalizedCategories = categories
        };

    public static EventsSectionViewModel CreateSavedSection(List<EventViewModel> events, List<CategoryDisplayItem> categories, string title) => new()
    {
        Title = title,
        Events = events,
        MaxItems = 18,
        ViewAllUrl = null,   // No dedicated favorites page yet
        LocalizedCategories = categories
    };
}
