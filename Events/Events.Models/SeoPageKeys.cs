using Events.Models.Enums;

namespace Events.Models;

// Well-known PageKey values for PageSeoMeta rows.
public static class SeoPageKeys
{
    public const string Home = "home";
    public const string AboutUs = "about-us";
    public const string Contact = "contact";
    public const string AllEvents = "events-all";
    public const string VenuesIndex = "venues-index";

    public static string ForCategory(EventCategory category) => $"category-{category.ToString().ToLowerInvariant()}";
}
