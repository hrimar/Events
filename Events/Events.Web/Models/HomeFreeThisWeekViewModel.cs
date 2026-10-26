namespace Events.Web.Models;

// Desktop homepage "Free this week" band (GS-REQ-007, 7.9): up to 6 compact cards in 3 columns x 2 rows.
public class HomeFreeThisWeekViewModel
{
    // The next 7 days (today plus 6), shown in the eyebrow.
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }

    // The cards show day and time ("Пт 25.09 · 21:30"), since the band covers several days.
    public List<EventCardViewModel> Cards { get; init; } = new();
}
