namespace Events.Models.Entities;

// One performance day of a submission; becomes one event on approval (GS-REQ-011, 11.6.4).
public class EventSubmissionDate
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }
    public EventSubmission Submission { get; set; } = null!;

    // Sofia date, no time part.
    public DateTime Date { get; set; }

    // Event created for this day; null before the approval or after the event was deleted.
    public int? EventId { get; set; }
}
