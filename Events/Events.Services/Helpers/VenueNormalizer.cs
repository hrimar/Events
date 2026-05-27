using System.Text;
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

        var transliterated = Transliterate(name.Trim());

        var slug = transliterated.ToLowerInvariant();
        slug = NonAlphanumericRegex.Replace(slug, " ");
        slug = MultipleWhitespaceRegex.Replace(slug, " ").Trim();
        slug = slug.Replace(' ', '-');

        return slug;
    }

    // Explicit Unicode code points are used to avoid ambiguity between visually identical
    // Cyrillic and Latin characters (e.g. Cyrillic '?' U+0410 vs Latin 'A' U+0041).
    private static readonly Dictionary<char, string> CyrillicMap = new()
    {
        { '\u0430', "a"   }, { '\u0410', "a"   }, // ? ?
        { '\u0431', "b"   }, { '\u0411', "b"   }, // ? ?
        { '\u0432', "v"   }, { '\u0412', "v"   }, // ? ?
        { '\u0433', "g"   }, { '\u0413', "g"   }, // ? ?
        { '\u0434', "d"   }, { '\u0414', "d"   }, // ? ?
        { '\u0435', "e"   }, { '\u0415', "e"   }, // ? ?
        { '\u0436', "zh"  }, { '\u0416', "zh"  }, // ? ?
        { '\u0437', "z"   }, { '\u0417', "z"   }, // ? ?
        { '\u0438', "i"   }, { '\u0418', "i"   }, // ? ?
        { '\u0439', "y"   }, { '\u0419', "y"   }, // ? ?
        { '\u043A', "k"   }, { '\u041A', "k"   }, // ? ?
        { '\u043B', "l"   }, { '\u041B', "l"   }, // ? ?
        { '\u043C', "m"   }, { '\u041C', "m"   }, // ? ?
        { '\u043D', "n"   }, { '\u041D', "n"   }, // ? ?
        { '\u043E', "o"   }, { '\u041E', "o"   }, // ? ?
        { '\u043F', "p"   }, { '\u041F', "p"   }, // ? ?
        { '\u0440', "r"   }, { '\u0420', "r"   }, // ? ?
        { '\u0441', "s"   }, { '\u0421', "s"   }, // ? ?
        { '\u0442', "t"   }, { '\u0422', "t"   }, // ? ?
        { '\u0443', "u"   }, { '\u0423', "u"   }, // ? ?
        { '\u0444', "f"   }, { '\u0424', "f"   }, // ? ?
        { '\u0445', "h"   }, { '\u0425', "h"   }, // ? ?
        { '\u0446', "ts"  }, { '\u0426', "ts"  }, // ? ?
        { '\u0447', "ch"  }, { '\u0427', "ch"  }, // ? ?
        { '\u0448', "sh"  }, { '\u0428', "sh"  }, // ? ?
        { '\u0449', "sht" }, { '\u0429', "sht" }, // ? ?
        { '\u044A', "a"   }, { '\u042A', "a"   }, // ? ?
        { '\u044C', ""    }, { '\u042C', ""    }, // ? ?
        { '\u044E', "yu"  }, { '\u042E', "yu"  }, // ? ?
        { '\u044F', "ya"  }, { '\u042F', "ya"  }, // ? ?
    };

    private static string Transliterate(string input)
    {
        var sb = new StringBuilder(input.Length * 2);

        foreach (var ch in input)
        {
            sb.Append(CyrillicMap.TryGetValue(ch, out var latin) ? latin : ch.ToString());
        }

        return sb.ToString();
    }
}
