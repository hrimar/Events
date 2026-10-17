namespace Events.Web.Models;

public class WeeklyEventsViewModel
{
    public List<DayEventsViewModel> ByDay { get; set; } = new();
    public List<CategoryEventsViewModel> ByCategory { get; set; } = new();
    public List<EventViewModel> WeekendEvents { get; set; } = new();
    public Dictionary<DateTime, int> CalendarCounts { get; set; } = new();
    public DateTime CalendarMonth { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    // Current calendar day in Europe/Sofia, set by the controller from ISofiaClock.
    public DateTime Today { get; set; }

    // Empty model anchored on the given Sofia "today" - used as a safe fallback on errors.
    public static WeeklyEventsViewModel Empty(DateTime today) => new()
    {
        Today = today,
        CalendarMonth = today,
        FromDate = today,
        ToDate = today.AddDays(6)
    };
}

public class DayEventsViewModel
{
    public DateTime Date { get; set; }
    public List<EventViewModel> Events { get; set; } = new();
    public bool IsToday { get; set; }
}

public class CategoryEventsViewModel
{
    public string CategoryKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public List<EventViewModel> Events { get; set; } = new();
}
