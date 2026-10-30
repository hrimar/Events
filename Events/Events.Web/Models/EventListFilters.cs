using Events.Models.Enums;
using Events.Services.Time;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models;

// The filters of "All events" and the options of their menus, shared by the period view and the paged list
// (GS-REQ-008, 8.2.4, 8.2.5). The route helpers build the links of the "When" chips and the filter buttons, so each
// one changes only its own value and keeps the others.
public class EventListFilters
{
    // Named period, or null for explicit dates or none.
    public EventPeriod? Period { get; set; }

    // Explicit dates; with a period they are not used in the links.
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Raw "category" query value (one name or a comma-separated list) and its names.
    public string? CurrentCategory { get; set; }
    public List<string> SelectedCategories { get; set; } = new();
    public string? SelectedSubCategory { get; set; }

    // Only "Free only" is offered (8.2.5); "Paid only" from older links is not kept.
    public bool IsFreeOnly { get; set; }
    public List<string> SelectedTags { get; set; } = new();

    // "With kids": events tagged for children (the tags of the "For kids" quick filter), 8.2.5.
    public bool IsKidsFilter { get; set; }

    // Search term from the header search; the paged list only.
    public string? SearchTerm { get; set; }

    // The period view has one order, by date and time, shown next to the filters (8.2.5).
    public bool HasFixedSort { get; set; }

    // Options of the category and subcategory menus; subcategories only when exactly one category is selected.
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
    public List<SelectListItem> AvailableSubCategories { get; set; } = new();

    // Both explicit dates and no period: the "Date" chip is the active one.
    public bool HasExplicitDates => !Period.HasValue && FromDate.HasValue && ToDate.HasValue;

    // Route values of the current period and filters.
    public Dictionary<string, string> FilterRoute()
    {
        var route = new Dictionary<string, string>();

        if (Period.HasValue)
        {
            route["period"] = EventPeriodCalculator.ToQueryValue(Period.Value);
        }
        else
        {
            if (FromDate.HasValue)
                route["fromDate"] = FromDate.Value.ToString("yyyy-MM-dd");

            if (ToDate.HasValue)
                route["toDate"] = ToDate.Value.ToString("yyyy-MM-dd");
        }

        if (!string.IsNullOrEmpty(CurrentCategory))
            route["category"] = CurrentCategory;

        if (!string.IsNullOrEmpty(SelectedSubCategory))
            route["subCategory"] = SelectedSubCategory;

        if (IsFreeOnly)
            route["free"] = "true";

        if (SelectedTags.Count > 0)
            route["tags"] = string.Join(",", SelectedTags);

        if (IsKidsFilter)
            route["kids"] = "true";

        if (!string.IsNullOrWhiteSpace(SearchTerm))
            route["search"] = SearchTerm;

        return route;
    }

    // The same filters for another named period; explicit dates are dropped.
    public Dictionary<string, string> RouteWithPeriod(EventPeriod period)
    {
        var route = RouteWithoutDates();
        route["period"] = EventPeriodCalculator.ToQueryValue(period);
        return route;
    }

    // The filters without the period and dates (the "Date" chip adds the picked days).
    public Dictionary<string, string> RouteWithoutDates()
    {
        var route = FilterRoute();
        route.Remove("period");
        route.Remove("fromDate");
        route.Remove("toDate");
        return route;
    }

    // The same period and filters with one value changed; null removes it. A new category also clears the
    // subcategory, which belongs to the previous category.
    public Dictionary<string, string> RouteWith(string key, string? value)
    {
        var route = FilterRoute();

        if (key == "category")
            route.Remove("subCategory");

        if (value is null)
            route.Remove(key);
        else
            route[key] = value;

        return route;
    }

    // The same filters with one tag added or removed (the tag cloud of the paged list).
    public Dictionary<string, string> RouteWithTagToggled(string tag)
    {
        var tags = SelectedTags.Contains(tag, StringComparer.OrdinalIgnoreCase)
            ? SelectedTags.Where(t => !string.Equals(t, tag, StringComparison.OrdinalIgnoreCase)).ToList()
            : SelectedTags.Append(tag).ToList();

        return RouteWith("tags", tags.Count > 0 ? string.Join(",", tags) : null);
    }

    // The filters and one day, for the "More N events" and deferred day requests (Events/PeriodDay, 8.2.8, 8.2.11).
    public Dictionary<string, string> RouteForDay(DateTime day)
    {
        var route = RouteWithoutDates();
        route.Remove("search");
        route["day"] = day.ToString("yyyy-MM-dd");
        return route;
    }
}
