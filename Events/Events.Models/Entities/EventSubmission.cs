using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

// An event proposed by an organizer through the "Add event" form (GS-REQ-011, 11.2, 11.7). Kept apart from Events,
// so nothing of it reaches the public site, search, sitemap, API or JSON-LD before an admin approves it.
public class EventSubmission
{
    public int Id { get; set; }

    // The organizer's account; status e-mails go to its address (11.2, "Имейл").
    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    // Language of the form ("bg" or "en"); the e-mails to the organizer use it.
    [Required]
    [MaxLength(5)]
    public string Culture { get; set; } = "bg";

    public EventSubmissionStatus Status { get; set; } = EventSubmissionStatus.Pending;

    public DateTime SubmittedAtUtc { get; set; }

    // Address the form was sent from (11.7).
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    // Event data (11.2).
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    // Optional English title; only a hint for the admin, never saved as Event.NameEn (user decision, 28.11.2026).
    [MaxLength(150)]
    public string? NameEn { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }

    // First day (Sofia date, no time part).
    public DateTime StartDate { get; set; }

    // Last day of a period; null for a one-day event.
    public DateTime? EndDate { get; set; }

    // Null when the organizer ticked "the time is not known yet".
    public TimeSpan? StartTime { get; set; }

    // Chosen from the venue list; null when the organizer typed a new venue.
    public int? CanonicalVenueId { get; set; }
    public CanonicalVenue? CanonicalVenue { get; set; }

    // Filled only when the venue is not in the list.
    [MaxLength(200)]
    public string? NewVenueName { get; set; }

    [MaxLength(300)]
    public string? NewVenueAddress { get; set; }

    // Plain text with line breaks, no HTML; encoded when the event is created.
    [Required]
    [MaxLength(3000)]
    public string Description { get; set; } = string.Empty;

    // Our copies in /media (GS-REQ-009, 9.1, 9.4): 1200px and 480px WebP.
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? TicketUrl { get; set; }

    public bool IsFree { get; set; }

    // Organizer data, never published (11.2, 11.7).
    [Required]
    [MaxLength(100)]
    public string OrganizerName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? OrganizerPhone { get; set; }

    [MaxLength(500)]
    public string? OrganizerWebsite { get; set; }

    // When the organizer ticked the consent box.
    public DateTime ConsentAtUtc { get; set; }

    // Reason shown to the organizer for a rejected submission.
    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    // Time of the approval, rejection or spam mark; the retention periods count from it (11.7).
    public DateTime? ReviewedAtUtc { get; set; }

    // The performance days in Sofia: one for a one-day event, the ticked days of a period otherwise. Each becomes
    // one event on approval (no EndDate on Event, user decision 28.11.2026).
    public ICollection<EventSubmissionDate> Dates { get; set; } = new List<EventSubmissionDate>();

    public ICollection<EventSubmissionAction> Actions { get; set; } = new List<EventSubmissionAction>();
}
