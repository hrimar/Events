using Events.Models.Enums;

namespace Events.Services.Time;

// Resolves a named period to an inclusive range of calendar days, relative to a given Sofia "today".
// Pure functions (today is a parameter, not read from a clock) so the homepage and "All events"
// share one definition and the rules can be tested for every day of the week.
public static class EventPeriodCalculator
{
    // Number of days in "next 7 days": today plus the following 6 days.
    public const int Next7Days = 7;

    // Wider window used when a filtered "next 7 days" list is empty (GS-REQ-007, 7.3.4).
    public const int FallbackDays = 30;

    private static readonly IReadOnlyDictionary<string, EventPeriod> QueryValues =
        new Dictionary<string, EventPeriod>(StringComparer.OrdinalIgnoreCase)
        {
            ["today"] = EventPeriod.Today,
            ["tomorrow"] = EventPeriod.Tomorrow,
            ["weekend"] = EventPeriod.Weekend,
            ["next7"] = EventPeriod.Next7
        };

    // Parses the "period" query value (today, tomorrow, weekend, next7); unknown values return false.
    public static bool TryParse(string? value, out EventPeriod period)
    {
        period = default;
        return !string.IsNullOrWhiteSpace(value) && QueryValues.TryGetValue(value.Trim(), out period);
    }

    // The value used in URLs, e.g. EventPeriod.Next7 -> "next7".
    public static string ToQueryValue(EventPeriod period) =>
        QueryValues.First(pair => pair.Value == period).Key;

    // First and last day (both inclusive, time 00:00) of the period.
    public static (DateTime From, DateTime To) GetRange(EventPeriod period, DateTime today)
    {
        var day = today.Date;

        return period switch
        {
            EventPeriod.Today => (day, day),
            EventPeriod.Tomorrow => (day.AddDays(1), day.AddDays(1)),
            EventPeriod.Weekend => GetWeekendRange(day),
            EventPeriod.Next7 => (day, day.AddDays(Next7Days - 1)),
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
        };
    }

    // Saturday and Sunday. Monday to Friday: the coming weekend; on Saturday: today and tomorrow;
    // on Sunday: only today (the glossary definition of "weekend").
    public static (DateTime From, DateTime To) GetWeekendRange(DateTime today)
    {
        var day = today.Date;

        if (day.DayOfWeek == DayOfWeek.Sunday)
            return (day, day);

        var daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)day.DayOfWeek + 7) % 7;
        var saturday = day.AddDays(daysUntilSaturday);
        return (saturday, saturday.AddDays(1));
    }

    // Today plus the following FallbackDays - 1 days.
    public static (DateTime From, DateTime To) GetFallbackRange(DateTime today) =>
        (today.Date, today.Date.AddDays(FallbackDays - 1));
}
