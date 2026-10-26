using Events.Web.Infrastructure;

namespace Events.Web.Models;

public class HomePageViewModel
{
    // Selects the homepage layout (_HomeDesktop or _HomeMobile), resolved on the server from the request.
    public DeviceClass DeviceClass { get; set; } = DeviceClass.Desktop;

    // Desktop hero (GS-REQ-007, 7.1-7.4); null for phones.
    public HomeHeroViewModel? Hero { get; set; }

    // Desktop "Tonight" row (GS-REQ-007, 7.5); null for phones or when there is nothing to show.
    public HomeTonightViewModel? Tonight { get; set; }

    // Desktop "This weekend" band (GS-REQ-007, 7.6); null for phones or when the weekend has no events.
    public HomeWeekendViewModel? Weekend { get; set; }

    // Desktop "Don't miss" cards (GS-REQ-007, 7.7); empty for phones or when there are no more featured events.
    public List<HomeFeatureCardViewModel> DontMiss { get; set; } = new();
    public WeeklyEventsViewModel WeeklyEvents { get; set; } = new();
    public EventsSectionViewModel SavedSection { get; set; } = new();
    public EventsSectionViewModel RecommendedSection { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int Next7DaysEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new();
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
}
