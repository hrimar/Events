using System.Text;
using System.Text.RegularExpressions;

namespace Events.Services.Helpers;

// Cyrillic to Latin by the Bulgarian Transliteration Act (e.g. Sofia, Shtastie). Used only for proper names and
// addresses: venue slugs, English venue names when NameEn is empty and the free-text location of an event without
// a venue (GS-REQ-010, 10.4). Never for event titles or descriptions, which are translated or left empty.
public static class CyrillicTransliterator
{
    // Cyrillic and Cyrillic Supplement blocks.
    private const char CyrillicFirst = '\u0400';
    private const char CyrillicLast = '\u052F';

    // A whole text without Cyrillic letters, for [RegularExpression] validation; the same pattern works in .NET and in
    // the browser (jQuery unobtrusive validation) (GS-REQ-010, 10.4).
    public const string NoCyrillicPattern = @"^[^\u0400-\u052F]*$";

    private const char LowercaseI = '\u0438';
    private const char LowercaseYa = '\u044F';

    // Established Latin spellings that differ from the letter-by-letter result, matched as whole words in any case.
    private static readonly (Regex Pattern, string Latin)[] Exceptions =
    {
        (new Regex("\\b\u0411\u044A\u043B\u0433\u0430\u0440\u0438\u044F\\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled), "Bulgaria"),
    };

    // Lowercase letters only; the case of the source letter is applied when writing. Explicit code points avoid
    // confusing Cyrillic letters with identical-looking Latin ones. Besides Bulgarian, the common Russian,
    // Ukrainian, Serbian and Macedonian letters are mapped; any other Cyrillic letter is dropped.
    private static readonly Dictionary<char, string> LowercaseMap = new()
    {
        { '\u0430', "a" },
        { '\u0431', "b" },
        { '\u0432', "v" },
        { '\u0433', "g" },
        { '\u0434', "d" },
        { '\u0435', "e" },
        { '\u0436', "zh" },
        { '\u0437', "z" },
        { '\u0438', "i" },
        { '\u0439', "y" },   // short i
        { '\u043A', "k" },
        { '\u043B', "l" },
        { '\u043C', "m" },
        { '\u043D', "n" },
        { '\u043E', "o" },
        { '\u043F', "p" },
        { '\u0440', "r" },
        { '\u0441', "s" },
        { '\u0442', "t" },
        { '\u0443', "u" },
        { '\u0444', "f" },
        { '\u0445', "h" },
        { '\u0446', "ts" },
        { '\u0447', "ch" },
        { '\u0448', "sh" },
        { '\u0449', "sht" },
        { '\u044A', "a" },   // hard sign
        { '\u044C', "y" },   // soft sign
        { '\u044E', "yu" },
        { '\u044F', "ya" },
        { '\u044B', "y" },   // Russian yery
        { '\u044D', "e" },   // Russian e
        { '\u0451', "yo" },  // Russian yo
        { '\u0454', "ye" },  // Ukrainian ye
        { '\u0456', "i" },   // Ukrainian i
        { '\u0457', "yi" },  // Ukrainian yi
        { '\u0491', "g" },   // Ukrainian ghe with upturn
        { '\u0452', "dj" },  // Serbian dje
        { '\u0453', "gj" },  // Macedonian gje
        { '\u0455', "dz" },  // Macedonian dze
        { '\u0458', "j" },   // Serbian je
        { '\u0459', "lj" },  // Serbian lje
        { '\u045A', "nj" },  // Serbian nje
        { '\u045B', "c" },   // Serbian tshe
        { '\u045C', "kj" },  // Macedonian kje
        { '\u045F', "dz" },  // Serbian dzhe
    };

    public static bool ContainsCyrillic(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        foreach (var ch in text)
        {
            if (IsCyrillic(ch))
                return true;
        }

        return false;
    }

    // Replaces every Cyrillic letter and keeps everything else. The exceptions above come first ("Bulgaria").
    // An uppercase letter gives a capitalized result ("Zh"), or a fully uppercase one inside an uppercase word
    // ("ZHAR"). By the Act, the letters "i" + "ya" at the end of a word give "ia" ("Sofia", not "Sofiya").
    // The result never contains Cyrillic.
    public static string ToLatin(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return text ?? string.Empty;

        if (!ContainsCyrillic(text))
            return text;

        foreach (var (pattern, latin) in Exceptions)
            text = pattern.Replace(text, match => IsAllUppercase(match.Value) ? latin.ToUpperInvariant() : latin);

        var sb = new StringBuilder(text.Length * 2);

        for (var i = 0; i < text.Length; i++)
        {
            var ch = text[i];

            if (!IsCyrillic(ch))
            {
                sb.Append(ch);
                continue;
            }

            var lower = char.ToLowerInvariant(ch);

            if (!LowercaseMap.TryGetValue(lower, out var latin))
                continue;

            if (lower == LowercaseYa && IsFinalIa(text, i))
                latin = "a";

            if (char.IsUpper(ch))
                latin = IsInUppercaseWord(text, i) ? latin.ToUpperInvariant() : char.ToUpperInvariant(latin[0]) + latin[1..];

            sb.Append(latin);
        }

        return sb.ToString();
    }

    private static bool IsCyrillic(char ch) => ch >= CyrillicFirst && ch <= CyrillicLast;

    private static bool IsAllUppercase(string word) => word.All(ch => !char.IsLetter(ch) || char.IsUpper(ch));

    // The letter "ya" after "i" at the end of a word.
    private static bool IsFinalIa(string text, int index)
    {
        var isAfterI = index > 0 && char.ToLowerInvariant(text[index - 1]) == LowercaseI;
        var isWordEnd = index + 1 >= text.Length || !char.IsLetter(text[index + 1]);

        return isAfterI && isWordEnd;
    }

    // The neighbouring letter decides: the next one, or the previous one at the end of a word. A single uppercase
    // letter stays capitalized ("Zh").
    private static bool IsInUppercaseWord(string text, int index)
    {
        if (index + 1 < text.Length && char.IsLetter(text[index + 1]))
            return char.IsUpper(text[index + 1]);

        return index > 0 && char.IsLetter(text[index - 1]) && char.IsUpper(text[index - 1]);
    }
}
