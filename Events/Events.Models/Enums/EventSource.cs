namespace Events.Models.Enums;

// Where an event came from (GS-REQ-011, 11.6.4). Only organizer submissions are marked for now; events from the
// crawler, the Excel import and the admin "Create Event" screen stay Unknown.
public enum EventSource
{
    Unknown = 0,

    // Created when an admin approved an "Add event" submission.
    OrganizerForm = 1
}
