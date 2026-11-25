using System.Text.RegularExpressions;

namespace Events.Services.Helpers;

public static class VenueNormalizer
{
    // Matches any character that is not a letter, digit, or whitespace
    private static readonly Regex NonAlphanumericRegex = new(@"[^\p{L}\p{N}\s]", RegexOptions.Compiled);

    // Matches two or more consecutive whitespace characters
    private static readonly Regex MultipleWhitespaceRegex = new(@"\s{2,}", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes a raw venue location string for alias matching.
    /// Applies lowercase, removes punctuation, and collapses whitespace.
    /// Returns null if the input is null or whitespace.
    /// </summary>
    public static string? Normalize(string? rawLocation)
    {
        if (string.IsNullOrWhiteSpace(rawLocation))
            return null;

        var normalized = rawLocation.Trim();

        normalized = NonAlphanumericRegex.Replace(normalized, " ");
        normalized = MultipleWhitespaceRegex.Replace(normalized, " ");
        normalized = normalized.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    /// <summary>
    /// Generates a URL-friendly slug from a venue name.
    /// Transliterates Cyrillic to Latin, lowercases, replaces spaces with hyphens,
    /// and removes any remaining non-alphanumeric characters.
    /// </summary>
    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Shared with the English venue names and the English event fields (GS-REQ-010, 10.4).
        var transliterated = CyrillicTransliterator.ToLatin(name.Trim());

        var slug = transliterated.ToLowerInvariant();
        slug = NonAlphanumericRegex.Replace(slug, " ");
        slug = MultipleWhitespaceRegex.Replace(slug, " ").Trim();
        slug = slug.Replace(' ', '-');

        return slug;
    }
}
