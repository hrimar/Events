namespace Events.Web.Models;

// Venue card (GS-REQ-006, 6.5): 3:2 photo, name and the number of upcoming events. Used on the homepage
// (GS-REQ-007, 7.10) and on the "Venues" page (GS-REQ-008, requester's decision). IsEager: the photo is in the
// first screen and loads at once; the others load on scroll.
public sealed record VenueCardViewModel(string Slug, string Name, string? PhotoUrl, int UpcomingEventsCount, bool IsEager = false);
