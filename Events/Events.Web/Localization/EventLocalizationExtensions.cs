using Events.Models.Entities;

namespace Events.Web.Localization;

// Event texts in the language of the page (GS-REQ-010, 10.4). An event without an English title shows the
// Bulgarian title and description on the English page, which is then not indexed.
public static class EventLocalizationExtensions
{
    public static bool HasEnglishTitle(this Event eventEntity) => !string.IsNullOrWhiteSpace(eventEntity.NameEn);

    public static string LocalizedName(this Event eventEntity, bool isEnglish) =>
        isEnglish && eventEntity.HasEnglishTitle() ? eventEntity.NameEn! : eventEntity.Name;

    // An English page with an English title shows only the English description, or none: no Bulgarian text next to
    // the English title (GS-REQ-010, 10.4). Without an English title the whole page stays Bulgarian.
    public static string? LocalizedDescription(this Event eventEntity, bool isEnglish)
    {
        if (!isEnglish || !eventEntity.HasEnglishTitle())
            return eventEntity.Description;

        return string.IsNullOrWhiteSpace(eventEntity.DescriptionEn) ? null : eventEntity.DescriptionEn;
    }

    // Bulgarian pages keep the crawled free-text location. English pages show the venue's English name, or the
    // transliterated free text when the event is not mapped to a venue.
    public static string LocalizedLocation(this Event eventEntity, bool isEnglish)
    {
        if (!isEnglish)
            return eventEntity.Location;

        return eventEntity.CanonicalVenue?.LocalizedName(isEnglish)
            ?? VenueLocalizationExtensions.LocalizeAddress(eventEntity.Location, isEnglish)!;
    }

    public static string LocalizedCity(this Event eventEntity, bool isEnglish) =>
        VenueLocalizationExtensions.LocalizeAddress(eventEntity.City, isEnglish)!;
}
