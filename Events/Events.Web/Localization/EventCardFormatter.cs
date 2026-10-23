using System.Globalization;
using Events.Web.Models;

namespace Events.Web.Localization;

// Texts shown on the event card (GS-REQ-006, 6.5).
public static class EventCardFormatter
{
    // Badge in the top left corner of the card image:
    // - under a day heading: "19:00";
    // - in lists without a day heading: "Пт 25.09 · 19:00" (EN "Fri 25/09 · 19:00"), or "Пт 25.09" without a time.
    // Returns null when there is nothing to show (no date requested and no known time), so no badge is rendered.
    public static string? FormatBadge(EventCardViewModel card, CultureInfo culture)
    {
        if (!card.ShowDate)
        {
            return card.TimeText;
        }

        var date = FormatShortDate(card.Date, culture);

        return card.HasKnownTime ? $"{date} · {card.TimeText}" : date;
    }

    // "Пт 25.09" / "Fri 25/09"
    public static string FormatShortDate(DateTime date, CultureInfo culture)
    {
        var isEnglish = culture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase);

        var dayName = culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek);
        if (dayName.Length > 0)
        {
            dayName = culture.TextInfo.ToUpper(dayName[0]) + dayName[1..];
        }

        // Day and month in the BG/EN numeric order from GS-BUG-004, without the year
        var dayMonth = date.ToString(isEnglish ? "dd'/'MM" : "dd'.'MM", CultureInfo.InvariantCulture);

        return $"{dayName} {dayMonth}";
    }
}
