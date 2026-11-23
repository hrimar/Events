namespace Events.Models.Enums;

// Result of the image step for one imported row (GS-REQ-009, 9.3.3).
public enum EventImportImageOutcome
{
    // Not processed yet by the background worker.
    Pending = 1,

    // A new local copy was stored (from the Image URL column or from the event page, see ImageFromPage).
    Downloaded = 2,

    // The same image content was already stored, so the existing copy is used (GS-REQ-009, 9.1.6).
    Reused = 3,

    // The row has neither Image URL nor Ticket URL: nothing to fetch.
    NoImage = 4,

    // The image could not be stored; the event keeps the default image (GS-REQ-009, 9.3.1).
    Failed = 5,

    // The row itself was not imported (missing required data), so there is no event to attach an image to.
    RowNotImported = 6
}
