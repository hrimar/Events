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

    // Desktop "Browse by interest" tiles (GS-REQ-007, 7.8); empty for phones or when no category has events.
    public List<HomeCategoryTileViewModel> Interests { get; set; } = new();

    // Desktop "Free this week" band (GS-REQ-007, 7.9); null for phones or with fewer than 3 free events.
    public HomeFreeThisWeekViewModel? FreeThisWeek { get; set; }

    // Desktop "Venues" (GS-REQ-007, 7.10); null for phones or when no venue qualifies.
    public HomeVenuesViewModel? Venues { get; set; }

    public WeeklyEventsViewModel WeeklyEvents { get; set; } = new();
    public EventsSectionViewModel SavedSection { get; set; } = new();
    public EventsSectionViewModel RecommendedSection { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int Next7DaysEvents { get; set; }
    public List<TagViewModel> PopularTags { get; set; } = new();
    public List<CategoryDisplayItem> LocalizedCategories { get; set; } = new();
}
