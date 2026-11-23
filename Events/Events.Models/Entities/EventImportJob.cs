using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

// One confirmed Excel/CSV import: the events are saved on confirm, then a background worker copies
// their images and fills the items. Kept in the database so the report and its CSV survive a restart
// (GS-REQ-009, 9.3.3, 9.3.4).
public class EventImportJob
{
    public int Id { get; set; }

    // Batch id from the Upload/Preview/Confirm steps (EventImportBatch.BatchId).
    public Guid BatchId { get; set; }

    [Required]
    [MaxLength(260)]
    public string FileName { get; set; } = string.Empty;

    // Admin user name who confirmed the import.
    [Required]
    [MaxLength(256)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public EventImportJobStatus Status { get; set; } = EventImportJobStatus.Queued;

    // All data rows in the file, including excluded ones (GS-REQ-009, 9.3.3, total rows).
    public int TotalRows { get; set; }

    // Row counts from the commit step (same meaning as EventImportCommitResult).
    public int CreatedCount { get; set; }

    public int SkippedExcludedCount { get; set; }

    public int FailedCount { get; set; }

    public ICollection<EventImportJobItem> Items { get; set; } = new List<EventImportJobItem>();
}
