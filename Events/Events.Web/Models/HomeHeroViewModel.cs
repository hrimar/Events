using Microsoft.AspNetCore.Routing;

namespace Events.Web.Models;

// Desktop homepage hero (GS-REQ-007, 7.1-7.4): title and quick filters on the left, featured carousel on the right.
public class HomeHeroViewModel
{
    // "Петък, 25 септември" / "Friday, 25 September"
    public string DateText { get; init; } = string.Empty;

    // H1 and subtitle from the admin "Site Content" screen; {broi7} is already replaced.
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;

    // Number shown in the "Today · N" chip.
    public int TodayCount { get; init; }

    // "What" chips with events in the next 30 days, in admin order.
    public List<HomeQuickFilterChipViewModel> QuickFilters { get; init; } = new();

    // Featured events (at most 5), or the single nearest upcoming event with an image when nothing is featured.
    public List<HomeHeroSlideViewModel> Slides { get; init; } = new();

    // False for the fallback slide: no "Featured" badge and no arrows (GS-REQ-007, 7.4.7).
    public bool HasFeaturedEvents { get; init; }
}

public class HomeQuickFilterChipViewModel
{
    public string Label { get; init; } = string.Empty;

    // Route values for /Events (period=next7 plus the chip's filter).
    public RouteValueDictionary RouteValues { get; init; } = new();

    // The "Free" chip has its own outline color (mockup).
    public bool IsFree { get; init; }
}

public class HomeHeroSlideViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;

    // "Сб 26 септември · 11:00 · Venue"; the time is left out when it is unknown.
    public string MetaText { get; init; } = string.Empty;
}
