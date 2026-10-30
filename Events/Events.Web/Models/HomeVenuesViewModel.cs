namespace Events.Web.Models;

// Desktop homepage "Venues" (GS-REQ-007, 7.10): the catalog size and 4 venue cards.
public class HomeVenuesViewModel
{
    // Number of venues in the catalog ("133 места в града", "Всички 133 места").
    public int TotalCount { get; init; }

    public List<HomeVenueCardViewModel> Venues { get; init; } = new();
}

// Venue card (GS-REQ-006, 6.5): 3:2 photo, name and the number of upcoming events in the next 30 days.
// Both names are kept, so the cached data serves both languages.
public class HomeVenueCardViewModel
{
    public string Slug { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string PhotoUrl { get; init; } = string.Empty;
    public int UpcomingEventsCount { get; init; }

    public string LocalizedName(bool isEnglish) => isEnglish && !string.IsNullOrWhiteSpace(NameEn) ? NameEn : Name;
}
