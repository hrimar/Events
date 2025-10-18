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

    // Sorting functionality
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";

    // Available categories for filter dropdown
    public static List<CategoryDisplayItem> AvailableCategories =>
        Enum.GetValues<EventCategory>()
            .Select(category => new CategoryDisplayItem
            {
                Value = category.ToString(),
                DisplayName = GetCategoryDisplayName(category),
                IconClass = GetCategoryIcon(category)
            })
            .ToList();

    // Sort options
    public static List<SortOption> AvailableSortOptions => new()
    {
        new() { Value = "date", DisplayName = "Date", IconClass = "fas fa-calendar" },
        new() { Value = "name", DisplayName = "Name", IconClass = "fas fa-sort-alpha-up" },
        new() { Value = "price", DisplayName = "Price", IconClass = "fas fa-dollar-sign" },
        new() { Value = "category", DisplayName = "Category", IconClass = "fas fa-tags" }
    };

    // Helper properties for UI
    public bool HasActiveFilters => !string.IsNullOrEmpty(CurrentCategory) ||
                                   IsFreeFilter.HasValue ||
                                   !string.IsNullOrEmpty(SearchTerm) ||
                                   FromDate.HasValue ||
                                   ToDate.HasValue ||
                                   !string.IsNullOrEmpty(SortBy);

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

            if (!string.IsNullOrEmpty(SortBy))
                filters.Add($"sortBy={SortBy}");

            if (!string.IsNullOrEmpty(SortOrder))
                filters.Add($"sortOrder={SortOrder}");

            return filters.Count > 0 ? string.Join("&", filters) : "";
        }
    }

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

public class SortOption
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
}