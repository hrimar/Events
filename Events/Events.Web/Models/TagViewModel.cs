using Events.Models.Enums;

namespace Events.Web.Models;

public class TagViewModel
{
    public string Name { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public EventCategory? Category { get; set; }
    public string DisplayName => Name;
    public string BadgeClass => GetBadgeClass();
    public string Size => GetSizeClass();

    private string GetBadgeClass()
    {
        if (EventCount >= 20) return "bg-primary";
        if (EventCount >= 10) return "bg-info";
        if (EventCount >= 5) return "bg-secondary";
        return "bg-light text-dark";
    }

    private string GetSizeClass()
    {
        if (EventCount >= 50) return "fs-4";
        if (EventCount >= 20) return "fs-5";
        if (EventCount >= 10) return "fs-6";
        return "";
    }

    public string CategoryIcon => Category switch
    {
        EventCategory.Music => "music",
        EventCategory.Sports => "trophy",
        EventCategory.Theatre => "drama",
        EventCategory.Cinema => "film",
        EventCategory.Art => "palette",
        EventCategory.Festivals => "party-popper",
        EventCategory.Exhibitions => "images",
        EventCategory.Conferences => "users",
        EventCategory.Workshops => "wrench",
        EventCategory.Entertainment => "martini",
        EventCategory.FoodDrink => "utensils",
        EventCategory.Markets => "store",
        _ => "tag"
    };
}