namespace Events.Web.Models.Admin;

public class AdminDashboardViewModel
{
    public int TotalEvents { get; set; }
    public int PublishedEvents { get; set; }
    public int DraftEvents { get; set; }
    public int FeaturedEvents { get; set; }
    public int UncategorizedEvents { get; set; }
    public int EventsAddedToday { get; set; }
    public int EventsAddedThisWeek { get; set; }
    public int EventsAddedThisMonth { get; set; }

    public List<CategoryStatistic> CategoryStatistics { get; set; } = new();
    public List<EventViewModel> RecentEvents { get; set; } = new();
    public List<EventViewModel> PendingCategorization { get; set; } = new();
}

public class CategoryStatistic
{
    public string CategoryName { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public int UncategorizedCount { get; set; }
}
