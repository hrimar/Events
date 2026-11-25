using Events.Models.Entities;
using Events.Services.Helpers;

namespace Events.Web.Localization;

// Picks the BG or EN venue name based on the current UI culture. An English page never shows the Bulgarian
// name: when NameEn is empty, the Bulgarian name is transliterated as a whole (GS-REQ-010, 10.4).
public static class VenueLocalizationExtensions
{
    public static string LocalizedName(this CanonicalVenue venue, bool isEnglish) =>
        LocalizeName(venue.Name, venue.NameEn, isEnglish);

    // Shared by the venue view models, which carry both names so cached data serves both languages.
    public static string LocalizeName(string name, string? nameEn, bool isEnglish)
    {
        if (!isEnglish)
            return name;

        return string.IsNullOrWhiteSpace(nameEn) ? CyrillicTransliterator.ToLatin(name) : nameEn;
    }

    // Addresses and city names are proper names: transliterated on English pages (GS-REQ-010, 10.4).
    public static string? LocalizeAddress(string? address, bool isEnglish) =>
        isEnglish && address is not null ? CyrillicTransliterator.ToLatin(address) : address;
}
