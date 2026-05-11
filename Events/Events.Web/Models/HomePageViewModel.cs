namespace Events.Web.Models;

public class HomePageViewModel
{
    public EventsSectionViewModel FeaturedSection { get; set; } = new();
    public EventsSectionViewModel SavedSection { get; set; } = new();
    // TODO: Add the RecommendedSection  here when the feature is implemented
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int Next7DaysEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new();
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
}
