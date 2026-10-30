using Events.Web.Models;

namespace Events.Web.Services;

// Filters and pages the events of the homepage "This weekend" section (GS-REQ-007, 7.6).
// A pure function over the cached weekend events, shared by the first render and the Home/WeekendEvents endpoint.
public static class HomeWeekendSelector
{
    // 4 columns x 2 rows; "Show more" adds the same number (GS-REQ-007, 7.6.5).
    public const int PageSize = 8;

    // Tags that mark children's events (same as the "For kids" quick filter, GS-REQ-007, 7.3).
    public static readonly IReadOnlyList<string> KidsTags = new[] { "For kids", "Family-friendly" };

    private static readonly TimeSpan MorningEnd = TimeSpan.FromHours(12);
    private static readonly TimeSpan EveningStart = TimeSpan.FromHours(18);

    private static readonly IReadOnlyDictionary<string, HomeWeekendFilter> QueryValues =
        new Dictionary<string, HomeWeekendFilter>(StringComparer.OrdinalIgnoreCase)
        {
            ["all"] = HomeWeekendFilter.All,
            ["morning"] = HomeWeekendFilter.Morning,
            ["evening"] = HomeWeekendFilter.Evening,
            ["kids"] = HomeWeekendFilter.Kids
        };

    // Query value of a filter chip, e.g. HomeWeekendFilter.Morning -> "morning".
    public static string ToQueryValue(HomeWeekendFilter filter) => QueryValues.First(pair => pair.Value == filter).Key;

    // Unknown or missing values mean "All".
    public static HomeWeekendFilter ParseFilter(string? value) =>
        !string.IsNullOrWhiteSpace(value) && QueryValues.TryGetValue(value.Trim(), out var filter) ? filter : HomeWeekendFilter.All;

    // One page of the day's events matching the filter, by start time.
    public static HomeWeekendCardsViewModel SelectPage(IEnumerable<EventViewModel> events, DateTime day, HomeWeekendFilter filter, int skip)
    {
        skip = Math.Max(0, skip);

        var matching = HomeEventOrdering
            .ByStartTime(events.Where(e => e.Date.Date == day.Date && Matches(e, filter)))
            .ToList();

        return new HomeWeekendCardsViewModel
        {
            Day = day.Date,

            // The tab already names the day, so the card badges show just the time (GS-REQ-006, 6.5.4).
            Cards = EventCardViewModel.FromEvents(matching.Skip(skip).Take(PageSize), showDate: false),
            Skip = skip,
            HasMore = matching.Count > skip + PageSize
        };
    }

    // Events without a known time match only "All" (GS-REQ-007, 7.6.4).
    private static bool Matches(EventViewModel eventModel, HomeWeekendFilter filter) => filter switch
    {
        HomeWeekendFilter.Morning => eventModel.HasKnownTime && HomeEventOrdering.GetStartTime(eventModel) < MorningEnd,
        HomeWeekendFilter.Evening => eventModel.HasKnownTime && HomeEventOrdering.GetStartTime(eventModel) >= EveningStart,
        HomeWeekendFilter.Kids => eventModel.Tags.Any(tag => KidsTags.Contains(tag, StringComparer.OrdinalIgnoreCase)),
        _ => true
    };
}
