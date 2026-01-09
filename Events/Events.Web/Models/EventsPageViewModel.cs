using Events.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models;

public class EventsPageViewModel
{
    public PaginatedList<EventViewModel> Events { get; set; } = new(new List<EventViewModel>(), 0, 1, 12);
    public string? CurrentCategory { get; set; }
    public string? SelectedSubCategory { get; set; }
    public bool? IsFreeFilter { get; set; }
    public string? SearchTerm { get; set; }
    public List<string> SelectedTags { get; set; } = new(); // Selected tags for filtering
    public List<TagViewModel> PopularTags { get; set; } = new(); // Popular tags for tag cloud
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string PageTitle { get; set; } = "All Events";

    public List<SelectListItem> AvailableSubCategories { get; set; } = new();

    // Sorting functionality
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";

    // Available categories for filter dropdown
    public static List<CategoryDisplayItem> AvailableCategories =>
        Enum.GetValues<EventCategory>()
            .Where(c => c != EventCategory.Undefined) // Don't show Undefined in filter
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
                                   !string.IsNullOrEmpty(SelectedSubCategory) ||
                                   IsFreeFilter.HasValue ||
                                   !string.IsNullOrEmpty(SearchTerm) ||
                                   SelectedTags.Any() || // Include tag filtering
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

            if (!string.IsNullOrEmpty(SelectedSubCategory))
                filters.Add($"subCategory={SelectedSubCategory}");

            if (IsFreeFilter.HasValue)
                filters.Add($"free={IsFreeFilter.Value.ToString().ToLower()}");

            if (!string.IsNullOrEmpty(SearchTerm))
                filters.Add($"search={Uri.EscapeDataString(SearchTerm)}");

            // Add tags to query string
            if (SelectedTags.Any())
                filters.Add($"tags={Uri.EscapeDataString(string.Join(",", SelectedTags))}");

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

    // Get popular tags grouped by category
    public Dictionary<EventCategory, List<TagViewModel>> PopularTagsByCategory =>
        PopularTags
            .Where(t => t.Category.HasValue)
            .GroupBy(t => t.Category!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

    // Get most popular tags (for quick access)
    public List<TagViewModel> TopTags => PopularTags
        .OrderByDescending(t => t.EventCount)
        .Take(10)
        .ToList();

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