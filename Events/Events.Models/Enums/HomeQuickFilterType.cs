namespace Events.Models.Enums;

// What a homepage "What" chip filters by (GS-REQ-007, 7.3).
public enum HomeQuickFilterType
{
    // Values are EventCategory names; an event matches any of them.
    Categories = 1,

    // Values are tag names; an event matches any of them.
    Tags = 2,

    // Free events only; Values is empty.
    Free = 3
}
