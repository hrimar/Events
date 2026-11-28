namespace Events.Models.Enums;

// Review state of an organizer "Add event" submission (GS-REQ-011, 11.6, 11.7).
public enum EventSubmissionStatus
{
    // Waiting for an admin; never visible on the public site.
    Pending = 0,

    // Published as one or more normal events.
    Approved = 1,

    // Not published; the organizer got an e-mail with the optional reason.
    Rejected = 2,

    // Not published, no e-mail; shown to the organizer as "Rejected" without a reason (11.4.3).
    Spam = 3
}
