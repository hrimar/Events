using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Services.Import;

// Counts and lists shown in the import report and its CSV (GS-REQ-009, 9.3.3). The image counts do not overlap:
// a stored image is "extracted from page" when it came from the Ticket URL page, otherwise "downloaded" (new copy)
// or "reused" (same content already stored, 9.1.6).
public sealed class EventImportReport
{
    public int TotalRows { get; private init; }
    public int CreatedCount { get; private init; }
    public int SkippedExcludedCount { get; private init; }
    public int NotImportedCount { get; private init; }

    public int Downloaded { get; private init; }
    public int ExtractedFromPage { get; private init; }
    public int Reused { get; private init; }
    public int NoImage { get; private init; }
    public int Failed { get; private init; }
    public int Pending { get; private init; }

    public bool IsCompleted { get; private init; }

    // Rows with an image to fetch, and how many of them are done (progress indicator, 9.3.4).
    public int ImagesToProcess => Downloaded + ExtractedFromPage + Reused + Failed + Pending;
    public int ImagesProcessed => ImagesToProcess - Pending;
    public int ProgressPercent => ImagesToProcess == 0 ? 100 : ImagesProcessed * 100 / ImagesToProcess;

    // Imported rows left without an image, for the manual upload from the report (9.3.5): failed rows, and rows a
    // completed job could not process (database error), shown as "Not processed".
    public IReadOnlyList<EventImportJobItem> RowsWithoutImage { get; private init; } = Array.Empty<EventImportJobItem>();

    public IReadOnlyList<EventImportJobItem> RowsNotImported { get; private init; } = Array.Empty<EventImportJobItem>();

    public static EventImportReport From(EventImportJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        var items = job.Items.OrderBy(i => i.RowNumber).ToList();
        var isCompleted = job.Status == EventImportJobStatus.Completed;

        return new EventImportReport
        {
            TotalRows = job.TotalRows,
            CreatedCount = job.CreatedCount,
            SkippedExcludedCount = job.SkippedExcludedCount,
            NotImportedCount = job.FailedCount,
            Downloaded = items.Count(i => i.Outcome == EventImportImageOutcome.Downloaded && !i.ImageFromPage),
            ExtractedFromPage = items.Count(i => IsStored(i) && i.ImageFromPage),
            Reused = items.Count(i => i.Outcome == EventImportImageOutcome.Reused && !i.ImageFromPage),
            NoImage = items.Count(i => i.Outcome == EventImportImageOutcome.NoImage),
            Failed = items.Count(i => i.Outcome == EventImportImageOutcome.Failed),
            Pending = items.Count(i => i.Outcome == EventImportImageOutcome.Pending),
            IsCompleted = isCompleted,
            RowsWithoutImage = items
                .Where(i => i.Outcome == EventImportImageOutcome.Failed || (isCompleted && i.Outcome == EventImportImageOutcome.Pending))
                .ToList(),
            RowsNotImported = items.Where(i => i.Outcome == EventImportImageOutcome.RowNotImported).ToList()
        };
    }

    // Result column of the report and the CSV.
    public static string GetResultLabel(EventImportJobItem item, bool jobCompleted) => item.Outcome switch
    {
        EventImportImageOutcome.Downloaded or EventImportImageOutcome.Reused when item.ImageFromPage => "Extracted from page",
        EventImportImageOutcome.Downloaded => "Downloaded",
        EventImportImageOutcome.Reused => "Reused",
        EventImportImageOutcome.NoImage => "No image in row",
        EventImportImageOutcome.Failed => "Failed",
        EventImportImageOutcome.RowNotImported => "Row not imported",
        EventImportImageOutcome.Pending => jobCompleted ? "Not processed" : "Pending",
        _ => item.Outcome.ToString()
    };

    public static string GetReasonLabel(ImageFetchFailureReason? reason) => reason switch
    {
        null => "",
        ImageFetchFailureReason.Timeout => "Timeout",
        ImageFetchFailureReason.NotFound => "Not found (404)",
        ImageFetchFailureReason.Expired => "Expired address",
        ImageFetchFailureReason.UnsupportedType => "Unsupported type",
        ImageFetchFailureReason.TooSmall => "Too small",
        ImageFetchFailureReason.ForbiddenAddress => "Forbidden address",
        ImageFetchFailureReason.TooLarge => "Too large",
        ImageFetchFailureReason.NotHttps => "Not https",
        ImageFetchFailureReason.InvalidUrl => "Invalid URL",
        ImageFetchFailureReason.NoImageOnPage => "No image on page",
        ImageFetchFailureReason.HttpError => "HTTP error",
        ImageFetchFailureReason.ProcessingError => "Processing error",
        _ => reason.Value.ToString()
    };

    private static bool IsStored(EventImportJobItem item) => item.Outcome is EventImportImageOutcome.Downloaded or EventImportImageOutcome.Reused;
}
