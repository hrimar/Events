using Events.Models.Enums;

namespace Events.Web.Models;

public class EventsPageViewModel
{
    public PaginatedList<EventViewModel> Events { get; set; } = new(new List<EventViewModel>(), 0, 1, 12);
    public string? CurrentCategory { get; set; }
    public bool? IsFreeFilter { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string PageTitle { get; set; } = "All Events";

    // Available categories from the enum
    public static List<CategoryDisplayItem> AvailableCategories =>
        Enum.GetValues<EventCategory>()
            .Select(category => new CategoryDisplayItem
            {
                Value = category.ToString(),
                DisplayName = GetCategoryDisplayName(category),
                IconClass = GetCategoryIcon(category)
            })
            .ToList();

    // Helper properties for UI
    public bool HasActiveFilters => !string.IsNullOrEmpty(CurrentCategory) ||
                                   IsFreeFilter.HasValue ||
                                   !string.IsNullOrEmpty(SearchTerm) ||
                                   FromDate.HasValue ||
                                   ToDate.HasValue;

    public string FiltersQueryString
    {
        get
        {
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(CurrentCategory))
                filters.Add($"category={CurrentCategory}");

            if (IsFreeFilter.HasValue)
                filters.Add($"free={IsFreeFilter.Value.ToString().ToLower()}");

            if (!string.IsNullOrEmpty(SearchTerm))
                filters.Add($"search={Uri.EscapeDataString(SearchTerm)}");

            if (FromDate.HasValue)
                filters.Add($"fromDate={FromDate.Value:yyyy-MM-dd}");

            if (ToDate.HasValue)
                filters.Add($"toDate={ToDate.Value:yyyy-MM-dd}");

            return filters.Count > 0 ? string.Join("&", filters) : "";
        }
    }

    // Helper methods for category display
    private static string GetCategoryDisplayName(EventCategory category) => category switch
    {
        EventCategory.Music => "Music",
        EventCategory.Art => "Art & Culture",
        EventCategory.Business => "Business",
        EventCategory.Sports => "Sports",
        EventCategory.Theatre => "Theatre",
        EventCategory.Cinema => "Cinema",
        EventCategory.Festivals => "Festivals",
        EventCategory.Exhibitions => "Exhibitions",
        EventCategory.Conferences => "Conferences",
        EventCategory.Workshops => "Workshops",
        _ => category.ToString()
    };

    private static string GetCategoryIcon(EventCategory category) => category switch
    {
        EventCategory.Music => "fas fa-music",
        EventCategory.Art => "fas fa-palette",
        EventCategory.Business => "fas fa-briefcase",
        EventCategory.Sports => "fas fa-running",
        EventCategory.Theatre => "fas fa-theater-masks",
        EventCategory.Cinema => "fas fa-film",
        EventCategory.Festivals => "fas fa-glass-cheers",
        EventCategory.Exhibitions => "fas fa-images",
        EventCategory.Conferences => "fas fa-users",
        EventCategory.Workshops => "fas fa-tools",
        _ => "fas fa-calendar"
    };
}

// Helper class for category display
public class CategoryDisplayItem
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
}