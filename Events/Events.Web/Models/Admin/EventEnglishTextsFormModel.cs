using System.ComponentModel.DataAnnotations;
using Events.Services.Helpers;

namespace Events.Web.Models.Admin;

// English event name and description of the admin event forms (Create and Edit), shown by the _EnglishTextFields partial
// (GS-REQ-010, 10.4). Both are optional: an upcoming event without an English title is translated by the crawler
// overnight. The English texts must not contain Cyrillic letters.
public abstract class EventEnglishTextsFormModel
{
    private const string NoCyrillicMessage = "English fields must not contain Cyrillic letters";

    [StringLength(200, ErrorMessage = "English event name cannot exceed 200 characters")]
    [RegularExpression(CyrillicTransliterator.NoCyrillicPattern, ErrorMessage = NoCyrillicMessage)]
    public string? NameEn { get; set; }

    [StringLength(4000, ErrorMessage = "English description cannot exceed 4000 characters")]
    [RegularExpression(CyrillicTransliterator.NoCyrillicPattern, ErrorMessage = NoCyrillicMessage)]
    public string? DescriptionEn { get; set; }

    // The values to store: trimmed, and null when empty (an empty English title means "translate automatically").
    public string? NameEnToSave => ToStoredValue(NameEn);

    public string? DescriptionEnToSave => ToStoredValue(DescriptionEn);

    private static string? ToStoredValue(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
