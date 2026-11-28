using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

// Who did what with a submission and when (GS-REQ-011, 11.6.7). The admin is stored by id and name without a
// foreign key, so the history stays when an admin account is deleted.
public class EventSubmissionAction
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public EventSubmission Submission { get; set; } = null!;

    public EventSubmissionActionType Action { get; set; }

    [Required]
    [MaxLength(450)]
    public string PerformedByUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PerformedBy { get; set; } = string.Empty;

    public DateTime PerformedAtUtc { get; set; }

    // Optional detail, e.g. the rejection reason.
    [MaxLength(500)]
    public string? Note { get; set; }
}
