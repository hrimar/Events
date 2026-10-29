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

        var dayName = Capitalize(culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek), culture);

        // Day and month in the BG/EN numeric order from GS-BUG-004, without the year
        var dayMonth = date.ToString(isEnglish ? "dd'/'MM" : "dd'.'MM", CultureInfo.InvariantCulture);

        return $"{dayName} {dayMonth}";
    }

    // "Сб 26 септември" / "Sat 26 September" (hero carousel meta line, GS-REQ-007, 7.4.4)
    public static string FormatDayAndMonthName(DateTime date, CultureInfo culture)
    {
        var dayName = Capitalize(culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek), culture);
        var monthName = culture.DateTimeFormat.GetMonthName(date.Month);

        return $"{dayName} {date.Day} {monthName}";
    }

    // "Петък, 25 септември" / "Friday, 25 September" (hero eyebrow, GS-REQ-007, 7.1.1)
    public static string FormatWeekdayAndDate(DateTime date, CultureInfo culture)
    {
        var dayName = Capitalize(culture.DateTimeFormat.GetDayName(date.DayOfWeek), culture);
        var monthName = culture.DateTimeFormat.GetMonthName(date.Month);

        return $"{dayName}, {date.Day} {monthName}";
    }

    // "Петък, 25 септември – четвъртък, 1 октомври" / "Friday, 25 September – Thursday, 1 October", or one day
    // (period view line under the H1, GS-REQ-008, 8.2.3). The second day name keeps the culture's own case.
    public static string FormatWeekdayAndDateRange(DateTime fromDate, DateTime toDate, CultureInfo culture)
    {
        if (fromDate.Date == toDate.Date)
            return FormatWeekdayAndDate(fromDate, culture);

        var format = culture.DateTimeFormat;
        var toText = $"{format.GetDayName(toDate.DayOfWeek)}, {toDate.Day} {format.GetMonthName(toDate.Month)}";

        return $"{FormatWeekdayAndDate(fromDate, culture)} – {toText}";
    }

    // "Петък" / "Friday" (section eyebrows, e.g. "Петък вечер", GS-REQ-007, 7.5.1)
    public static string FormatWeekday(DateTime date, CultureInfo culture) =>
        Capitalize(culture.DateTimeFormat.GetDayName(date.DayOfWeek), culture);

    // "Събота, 31 окт" / "Saturday, 31 Oct" (weekend day tabs, GS-REQ-007, 7.6.3). The month is shortened to
    // its first 3 letters, because the abbreviations from the culture data differ between ICU versions ("окт.").
    public static string FormatWeekdayAndShortMonth(DateTime date, CultureInfo culture)
    {
        var monthName = culture.DateTimeFormat.GetMonthName(date.Month);
        var shortMonth = monthName.Length > 3 ? monthName[..3] : monthName;

        return $"{FormatWeekday(date, culture)}, {date.Day} {shortMonth}";
    }

    // "26–27 септември", "31 октомври – 1 ноември" or "1 ноември" for a single day
    // (weekend eyebrow, GS-REQ-007, 7.6.2)
    public static string FormatDayRange(DateTime fromDate, DateTime toDate, CultureInfo culture)
    {
        var format = culture.DateTimeFormat;

        if (fromDate.Date == toDate.Date)
            return $"{toDate.Day} {format.GetMonthName(toDate.Month)}";

        if (fromDate.Month == toDate.Month)
            return $"{fromDate.Day}–{toDate.Day} {format.GetMonthName(toDate.Month)}";

        return $"{fromDate.Day} {format.GetMonthName(fromDate.Month)} – {toDate.Day} {format.GetMonthName(toDate.Month)}";
    }

    private static string Capitalize(string text, CultureInfo culture) =>
        text.Length > 0 ? culture.TextInfo.ToUpper(text[0]) + text[1..] : text;
}
