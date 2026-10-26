namespace Events.Web.Models;

// In-place filters of the "This weekend" section (GS-REQ-007, 7.6.4).
public enum HomeWeekendFilter
{
    All = 1,

    // Start before 12:00.
    Morning = 2,

    // Start from 18:00.
    Evening = 3,

    // Tag "For kids" or "Family-friendly".
    Kids = 4
}

// Desktop homepage "This weekend" band: day tabs, filter chips and an 8-card grid (GS-REQ-007, 7.6).
public class HomeWeekendViewModel
{
    // First and last day of the weekend (a single day on Sunday).
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }

    // One tab per day: Saturday and Sunday, or only Sunday on a Sunday.
    public List<DateTime> Days { get; init; } = new();

    // First page of the first day with the "All" filter, rendered with the page.
    public HomeWeekendCardsViewModel FirstPage { get; init; } = new();
}

// One page of weekend cards, also returned by Home/WeekendEvents for the tabs, chips and "Show more".
public class HomeWeekendCardsViewModel
{
    public DateTime Day { get; init; }
    public List<EventCardViewModel> Cards { get; init; } = new();

    // Number of cards before this page; 0 for the first page.
    public int Skip { get; init; }

    public bool HasMore { get; init; }
}
