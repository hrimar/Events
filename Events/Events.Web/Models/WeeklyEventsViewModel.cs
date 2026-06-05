namespace Events.Web.Models;

public class WeeklyEventsViewModel
{
    public List<DayEventsViewModel> ByDay { get; set; } = new();
    public List<CategoryEventsViewModel> ByCategory { get; set; } = new();
    public List<EventViewModel> WeekendEvents { get; set; } = new();
    public Dictionary<DateTime, int> CalendarCounts { get; set; } = new();
    public DateTime CalendarMonth { get; set; } = DateTime.Today;
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today.AddDays(6);
}

public class DayEventsViewModel
{
    public DateTime Date { get; set; }
    public List<EventViewModel> Events { get; set; } = new();
    public bool IsToday => Date.Date == DateTime.Today;
}

public class CategoryEventsViewModel
{
    public string CategoryKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public List<EventViewModel> Events { get; set; } = new();
}
