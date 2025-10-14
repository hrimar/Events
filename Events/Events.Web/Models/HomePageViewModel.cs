namespace Events.Web.Models;

public class HomePageViewModel
{
    public List<EventViewModel> Events { get; set; } = new();
    public int TotalEvents { get; set; }

    public PaginatedList<EventViewModel> PaginatedEvents { get; set; } = null!;

    // optional statistics
    public int TodayEvents { get; set; }
    public int ThisWeekEvents { get; set; }
}