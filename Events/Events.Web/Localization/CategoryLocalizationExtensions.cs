using Events.Models.Enums;
using Events.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Events.Web.Localization;

/// <summary>
/// Extension methods that translate EventCategory and all SubCategory enums
/// using the shared IStringLocalizer. Resource keys follow the pattern:
///   Category_{EnumName}     e.g. Category_Music
///   SubCategory_{EnumName}  e.g. SubCategory_Pop
///
/// These extensions exist in Events.Web because EventCategory is defined in Events.Models, which has no dependency on Events.Web or IStringLocalizer.
/// Placing the logic here keeps the Models project clean and avoids circular references.
/// </summary>
public static class CategoryLocalizationExtensions
{
    // Categories
    public static string Localize(this EventCategory category, IStringLocalizer<SharedResources> localizer)
        => localizer[$"Category_{category}"];

    public static string LocalizeIcon(this EventCategory category) => category switch
    {
        EventCategory.Music       => "fas fa-music",
        EventCategory.Art         => "fas fa-palette",
        EventCategory.Business    => "fas fa-briefcase",
        EventCategory.Sports      => "fas fa-running",
        EventCategory.Theatre     => "fas fa-theater-masks",
        EventCategory.Cinema      => "fas fa-film",
        EventCategory.Festivals   => "fas fa-glass-cheers",
        EventCategory.Exhibitions => "fas fa-images",
        EventCategory.Conferences => "fas fa-users",
        EventCategory.Workshops   => "fas fa-tools",
        _                         => "fas fa-calendar"
    };

    // SubCategory enums - each follows the same pattern: SubCategory_{EnumValue}
    public static string Localize(this MusicSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this ArtSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this BusinessSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this SportsSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this TheatreSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this CinemaSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this FestivalsSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this ExhibitionsSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this ConferencesSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];

    public static string Localize(this WorkshopsSubCategory sub, IStringLocalizer<SharedResources> localizer)
        => localizer[$"SubCategory_{sub}"];
}

