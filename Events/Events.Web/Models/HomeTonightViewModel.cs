namespace Events.Web.Models;

// Which day the "Tonight" row shows (GS-REQ-007, 7.5).
public enum HomeTonightDay
{
    // Today from 16:00: eyebrow "Петък вечер", heading "Довечера".
    Tonight = 1,

    // Today before 16:00: eyebrow "Петък", heading "Днес".
    Today = 2,

    // Fewer than 5 of today's events are still ahead: tomorrow's events, heading "Утре".
    Tomorrow = 3
}

// Desktop homepage "Tonight" row: up to 20 standard cards, 5 visible at a time.
public class HomeTonightViewModel
{
    public HomeTonightDay Day { get; init; }

    // The listed day (today or tomorrow), for the weekday in the eyebrow.
    public DateTime Date { get; init; }

    // All events of that day, for "Всички N за днес" / "Всички N за утре".
    public int TotalCount { get; init; }

    public List<EventCardViewModel> Cards { get; init; } = new();
}
