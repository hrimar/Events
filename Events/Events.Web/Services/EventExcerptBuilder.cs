using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Events.Web.Services;

// Short description for the "Don't miss" cards (GS-REQ-007, 7.7.4): the start of the event description,
// about 140 characters, cut at a sentence or word boundary. Descriptions that only repeat the title and the
// date (e.g. "25 септември ЧЕРНА КОМЕДИЯ") give no text.
public static class EventExcerptBuilder
{
    public const int MaxLength = 140;

    // A sentence end is used as the cut only if the excerpt is at least this long; otherwise the cut is at a word.
    private const int MinSentenceExcerptLength = 60;

    // Fewer letters than this outside the title and date words means the description adds nothing.
    private const int MinOwnLetters = 15;

    private const string Ellipsis = "…";

    private static readonly Regex HtmlTagPattern = new("<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex WhitespacePattern = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex WordPattern = new(@"\p{L}+|\d+", RegexOptions.Compiled);
    private static readonly char[] SentenceEnds = { '.', '!', '?' };

    // Month and day names (full and short) in both site languages, for the "repeats the date" check.
    private static readonly HashSet<string> DateWords = BuildDateWords(new CultureInfo("bg-BG"), new CultureInfo("en-US"));

    // Returns null when there is nothing worth showing.
    public static string? Build(string? description, string eventName)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        var text = WebUtility.HtmlDecode(HtmlTagPattern.Replace(description, " "));
        text = WhitespacePattern.Replace(text, " ").Trim();

        if (text.Length == 0 || OnlyRepeatsTitleAndDate(text, eventName))
            return null;

        return text.Length <= MaxLength ? text : Shorten(text);
    }

    private static string Shorten(string text)
    {
        var window = text[..(MaxLength + 1)];

        // A whole sentence (or several) reads better than a cut one, and needs no ellipsis.
        var sentenceEnd = window.LastIndexOfAny(SentenceEnds, MaxLength - 1);
        while (sentenceEnd >= MinSentenceExcerptLength - 1)
        {
            if (sentenceEnd + 1 < window.Length && window[sentenceEnd + 1] == ' ')
                return text[..(sentenceEnd + 1)];

            sentenceEnd = window.LastIndexOfAny(SentenceEnds, sentenceEnd - 1);
        }

        var lastSpace = window.LastIndexOf(' ');
        var cut = lastSpace > 0 ? text[..lastSpace] : text[..MaxLength];

        return cut.TrimEnd(' ', ',', ';', ':', '-', '–', '—') + Ellipsis;
    }

    private static bool OnlyRepeatsTitleAndDate(string text, string eventName)
    {
        var titleWords = WordPattern.Matches(eventName)
            .Select(m => m.Value.ToLowerInvariant())
            .ToHashSet();

        var ownLetters = WordPattern.Matches(text)
            .Select(m => m.Value.ToLowerInvariant())
            .Where(word => !char.IsDigit(word[0]) && !titleWords.Contains(word) && !DateWords.Contains(word))
            .Sum(word => word.Length);

        return ownLetters < MinOwnLetters;
    }

    private static HashSet<string> BuildDateWords(params CultureInfo[] cultures)
    {
        var words = new HashSet<string>();
        foreach (var format in cultures.Select(c => c.DateTimeFormat))
        {
            var names = format.MonthNames
                .Concat(format.MonthGenitiveNames)
                .Concat(format.AbbreviatedMonthNames)
                .Concat(format.DayNames)
                .Concat(format.AbbreviatedDayNames);

            foreach (var name in names.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                foreach (Match match in WordPattern.Matches(name))
                    words.Add(match.Value.ToLowerInvariant());
            }
        }

        return words;
    }
}
