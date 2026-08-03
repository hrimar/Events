namespace Events.Services.Import.Models;

/// <summary>
/// A parsed and validated import file, held in cache between the Upload/Preview/Confirm steps
/// while the admin reviews and corrects flagged rows.
/// </summary>
public class EventImportBatch
{
    public Guid BatchId { get; set; } = Guid.NewGuid();

    public string OriginalFileName { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<ImportRowResult> Rows { get; set; } = new();
}

public class EventImportCommitResult
{
    public int CreatedCount { get; set; }

    public int SkippedExcludedCount { get; set; }

    public int FailedCount { get; set; }

    public List<int> CreatedEventIds { get; set; } = new();

    public List<(int RowNumber, string Error)> Failures { get; set; } = new();
}
