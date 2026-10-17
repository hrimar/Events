using Events.Models.Entities;

namespace Events.Web.Localization;

// Picks the BG or EN venue name based on the current UI culture (EN falls back to BG when empty).
public static class VenueLocalizationExtensions
{
    public static string LocalizedName(this CanonicalVenue venue, bool isEnglish) =>
        isEnglish && !string.IsNullOrWhiteSpace(venue.NameEn) ? venue.NameEn : venue.Name;
}
