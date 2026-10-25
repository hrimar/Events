using Events.Web.Infrastructure;

namespace Events.Web.Models;

public class HomePageViewModel
{
    // Selects the homepage layout (_HomeDesktop or _HomeMobile), resolved on the server from the request.
    public DeviceClass DeviceClass { get; set; } = DeviceClass.Desktop;

    public string HeroTitle { get; set; } = string.Empty;
    public string HeroSubtitle { get; set; } = string.Empty;
    public WeeklyEventsViewModel WeeklyEvents { get; set; } = new();
    public EventsSectionViewModel SavedSection { get; set; } = new();
    public EventsSectionViewModel RecommendedSection { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int Next7DaysEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new();
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
}
