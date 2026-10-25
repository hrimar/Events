namespace Events.Models.Enums;

// Named date periods used by the "When" chips on the homepage and the "period" query parameter
// of "All events" (/Events?period=next7). The server resolves each period to calendar dates in Europe/Sofia.
public enum EventPeriod
{
    Today = 1,
    Tomorrow = 2,
    Weekend = 3,
    Next7 = 4
}
