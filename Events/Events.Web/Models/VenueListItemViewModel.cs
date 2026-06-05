using System.Globalization;

namespace Events.Web.Models;

public class VenueListItemViewModel
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public int UpcomingEventsCount { get; set; }

    public string DisplayName => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en" && !string.IsNullOrEmpty(NameEn) ? NameEn : Name;
}
