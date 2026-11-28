namespace Events.Models.Enums;

// Admin action on an organizer submission, kept with who did it and when (GS-REQ-011, 11.6.7).
public enum EventSubmissionActionType
{
    Edited = 1,
    Approved = 2,
    Rejected = 3,
    MarkedSpam = 4
}
