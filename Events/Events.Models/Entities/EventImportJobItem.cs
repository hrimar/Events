using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

// Image result for one imported row; one report line (GS-REQ-009, 9.3.3).
public class EventImportJobItem
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public EventImportJob Job { get; set; } = null!;

    // Row number in the source file, as shown on the Preview screen.
    public int RowNumber { get; set; }

    // Created event; null when the row was not imported or the event was deleted later.
    public int? EventId { get; set; }

    [MaxLength(200)]
    public string EventName { get; set; } = string.Empty;

    // "Image URL" column, untruncated (GS-REQ-009, 9.1).
    [MaxLength(2000)]
    public string? SourceImageUrl { get; set; }

    // "Ticket URL" column, used to look for an image when "Image URL" is empty (GS-REQ-009, 9.2).
    [MaxLength(2000)]
    public string? PageUrl { get; set; }

    // True when the stored image was found on the PageUrl page (GS-REQ-009, 9.3.3, extracted from page).
    public bool ImageFromPage { get; set; }

    public EventImportImageOutcome Outcome { get; set; } = EventImportImageOutcome.Pending;

    // Set only when Outcome is Failed.
    public ImageFetchFailureReason? FailureReason { get; set; }

    // Short technical detail for the admin (HTTP status, size, error message).
    [MaxLength(500)]
    public string? Detail { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }
}
