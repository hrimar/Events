using Events.Models.Enums;
using Events.Services.Time;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models;

// "All events" for a period, grouped by day (GS-REQ-008, 8.2). Shown when the address has "period" or both
// "fromDate" and "toDate"; without them the paged list (EventsPageViewModel) is used.
public class EventsPeriodPageViewModel
{
    // H1 ("Следващите 7 дни в София", "Бизнес в София: следващите 7 дни", "Събития на 3 октомври", 8.2.2).
    public string Heading { get; set; } = string.Empty;

    // Line under the H1: the period's days and the number of events (8.2.3).
    public string DateRangeText { get; set; } = string.Empty;
    public string TotalCountText { get; set; } = string.Empty;
    public int TotalCount { get; set; }

    // Days with events, in chronological order (8.2.6, 8.2.9).
    public List<EventsPeriodDayViewModel> Days { get; set; } = new();

    // First and last day listed (the 30-day range when an empty "next 7 days" was widened, GS-REQ-007, 7.3.4).
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    // Named period, or null for explicit dates.
    public EventPeriod? Period { get; set; }

    // Set when an empty "next 7 days" list was widened to 30 days (GS-REQ-007, 7.3.4).
    public string? PeriodFallbackMessage { get; set; }

    // Filters kept by the period chips and filter buttons (8.2.4, 8.2.5).
    public string? CurrentCategory { get; set; }
    public List<string> SelectedCategories { get; set; } = new();
    public string? SelectedSubCategory { get; set; }
    public bool? IsFreeFilter { get; set; }
    public List<string> SelectedTags { get; set; } = new();

    // "With kids": events tagged for children (the tags of the "For kids" quick filter), 8.2.5.
    public bool IsKidsFilter { get; set; }

    // Options of the category and subcategory menus; subcategories only when exactly one category is selected.
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
    public List<SelectListItem> AvailableSubCategories { get; set; } = new();

    public string? PageMetaDescription { get; set; }

    // Query value of the selected period ("next7"), or null.
    public string? PeriodQueryValue => Period.HasValue ? EventPeriodCalculator.ToQueryValue(Period.Value) : null;

    // Route values of the current period and filters, so a chip or a filter changes only its own value
    // and keeps the others (8.2.4, 8.2.5).
    public Dictionary<string, string> FilterRoute()
    {
        var route = new Dictionary<string, string>();

        if (Period.HasValue)
        {
            route["period"] = PeriodQueryValue!;
        }
        else
        {
            route["fromDate"] = FromDate.ToString("yyyy-MM-dd");
            route["toDate"] = ToDate.ToString("yyyy-MM-dd");
        }

        if (!string.IsNullOrEmpty(CurrentCategory))
            route["category"] = CurrentCategory;

        if (!string.IsNullOrEmpty(SelectedSubCategory))
            route["subCategory"] = SelectedSubCategory;

        if (IsFreeFilter == true)
            route["free"] = "true";

        if (SelectedTags.Count > 0)
            route["tags"] = string.Join(",", SelectedTags);

        if (IsKidsFilter)
            route["kids"] = "true";

        return route;
    }

    // The same filters for another named period; explicit dates are dropped.
    public Dictionary<string, string> RouteWithPeriod(EventPeriod period)
    {
        var route = FilterRoute();
        route.Remove("fromDate");
        route.Remove("toDate");
        route["period"] = EventPeriodCalculator.ToQueryValue(period);
        return route;
    }

    // The filters without the period and dates (the "Date" chip adds the picked day).
    public Dictionary<string, string> RouteWithoutDates()
    {
        var route = FilterRoute();
        route.Remove("period");
        route.Remove("fromDate");
        route.Remove("toDate");
        return route;
    }

    // The filters and one day, for the "More N events" request of that day (Events/PeriodDay, 8.2.8).
    public Dictionary<string, string> RouteForDay(DateTime day)
    {
        var route = RouteWithoutDates();
        route["day"] = day.ToString("yyyy-MM-dd");
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
}

// One day of the period view: heading, "Today"/"Tomorrow" label, event count and up to 8 cards (8.2.6, 8.2.8).
public class EventsPeriodDayViewModel
{
    public DateTime Day { get; set; }

    // "Петък, 25 септември" (GS-REQ-006, 6.3)
    public string Heading { get; set; } = string.Empty;

    // "Днес" (accent) or "Утре" (tag colour); null on other days.
    public string? DayLabel { get; set; }
    public bool IsToday { get; set; }

    // "33 събития"
    public string CountText { get; set; } = string.Empty;
    public int TotalCount { get; set; }

    // Cards show only the time, the date is in the day heading.
    public List<EventCardViewModel> Cards { get; set; } = new();

    // Events of the day not shown yet ("More N events on Friday", 8.2.8).
    public int RemainingCount => TotalCount - Cards.Count;

    // Text of the "More" button ("Още 25 събития в петък"); null when all the day's events are shown.
    public string? MoreText { get; set; }

    // The cards of this day load on scroll (more than 200 events in the period, 8.2.11).
    public bool IsDeferred { get; set; }

    // Route values of this day's requests: the page filters and the day (Events/PeriodDay, Events/PeriodDaySection).
    public Dictionary<string, string> Route { get; set; } = new();

    // "All events" for this day alone with the same filters: the link of a deferred day without JavaScript.
    public Dictionary<string, string> DayPageRoute()
    {
        var route = new Dictionary<string, string>(Route);
        route.Remove("day");
        route["fromDate"] = Day.ToString("yyyy-MM-dd");
        route["toDate"] = Day.ToString("yyyy-MM-dd");
        return route;
    }
}
