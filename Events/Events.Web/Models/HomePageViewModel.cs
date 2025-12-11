namespace Events.Web.Models;

public class HomePageViewModel
{
    public List<EventViewModel> FeaturedEvents { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int ThisWeekEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new(); // Popular tags for homepage
}