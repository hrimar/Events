using Events.Models.Enums;
using Events.Web.Localization;
using Events.Web.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

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
    public string PageTitle { get; set; } = string.Empty;

    public List<SelectListItem> AvailableSubCategories { get; set; } = new();

    // Pre-built localized lists - populated by the controller
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
    public List<SortOption> LocalizedSortOptions { get; set; } = new();

    // Sorting functionality
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc";

    // Available categories - localized (preferred, call from controllers/views with localizer)
    public static List<CategoryDisplayItem> GetAvailableCategories(IStringLocalizer<SharedResources> localizer) =>
        Enum.GetValues<EventCategory>()
            .Where(c => c != EventCategory.Undefined) // Don't show Undefined in filter
            .Select(category => new CategoryDisplayItem
            {
                Value = category.ToString(),
                DisplayName = category.Localize(localizer),
                IconClass = category.LocalizeIcon()
            })
            .ToList();

    // Available categories - static fallback (Bulgarian, used when localizer is not available)
    public static List<CategoryDisplayItem> AvailableCategories =>
        Enum.GetValues<EventCategory>()
            .Where(c => c != EventCategory.Undefined)
            .Select(category => new CategoryDisplayItem
            {
                Value = category.ToString(),
                DisplayName = GetCategoryDisplayName(category),
                IconClass = category.LocalizeIcon()
            })
            .ToList();

    // Sort options - localized
    public static List<SortOption> GetAvailableSortOptions(IStringLocalizer<SharedResources> localizer) => new()
    {
        new() { Value = "date",        DisplayName = localizer["Sort_Date"],        IconClass = "fas fa-calendar" },
        new() { Value = "name",        DisplayName = localizer["Sort_Name"],        IconClass = "fas fa-sort-alpha-up" },
        new() { Value = "price",       DisplayName = localizer["Sort_Price"],       IconClass = "fas fa-dollar-sign" },
        new() { Value = "category",    DisplayName = localizer["Sort_Category"],    IconClass = "fas fa-tags" },
        new() { Value = "subcategory", DisplayName = localizer["Sort_Subcategory"], IconClass = "fas fa-layer-group" }
    };

    // Sort options - static fallback (Bulgarian)
    public static List<SortOption> AvailableSortOptions => new()
    {
        new() { Value = "date",        DisplayName = "Date",        IconClass = "fas fa-calendar" },
        new() { Value = "name",        DisplayName = "Name",        IconClass = "fas fa-sort-alpha-up" },
        new() { Value = "price",       DisplayName = "Price",       IconClass = "fas fa-dollar-sign" },
        new() { Value = "category",    DisplayName = "Category",    IconClass = "fas fa-tags" },
        new() { Value = "subcategory", DisplayName = "Subcategory", IconClass = "fas fa-layer-group" }
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
        EventCategory.Music       => "Музика",
        EventCategory.Art         => "Изкуство и Култура",
        EventCategory.Business    => "Бизнес",
        EventCategory.Sports      => "Спорт",
        EventCategory.Theatre     => "Театър",
        EventCategory.Cinema      => "Кино",
        EventCategory.Festivals   => "Фестивали",
        EventCategory.Exhibitions => "Изложби",
        EventCategory.Conferences => "Конференции",
        EventCategory.Workshops   => "Уъркшопи",
        _                         => category.ToString()
    };
}

public class SortOption
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
}