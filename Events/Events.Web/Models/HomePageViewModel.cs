namespace Events.Web.Models;

public class HomePageViewModel
{
    public EventsSectionViewModel FeaturedSection { get; set; } = new();
    public EventsSectionViewModel SavedSection { get; set; } = new();
    public EventsSectionViewModel RecommendedSection { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int Next7DaysEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new();
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
}
