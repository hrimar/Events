using System.Text;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Import;
using Events.Services.Time;

namespace Events.Services.Tests.Import;

// Import report counts and CSV (GS-REQ-009, 9.3.3).
public class EventImportReportTests
{
    private static EventImportJobItem Item(int row, EventImportImageOutcome outcome, bool fromPage = false,
        ImageFetchFailureReason? reason = null, int? eventId = 1) => new()
        {
            RowNumber = row,
            EventId = eventId,
            EventName = $"Event {row}",
            Outcome = outcome,
            ImageFromPage = fromPage,
            FailureReason = reason
        };

    private static EventImportJob Job(EventImportJobStatus status, params EventImportJobItem[] items) => new()
    {
        Id = 3,
        FileName = "fb.xlsx",
        CreatedBy = "admin",
        Status = status,
        TotalRows = 20,
        CreatedCount = 18,
        SkippedExcludedCount = 1,
        FailedCount = 1,
        Items = items.ToList()
    };

    [Fact]
    public void From_CountsDoNotOverlap()
    {
        var job = Job(EventImportJobStatus.Completed,
            Item(1, EventImportImageOutcome.Downloaded),
            Item(2, EventImportImageOutcome.Downloaded),
            Item(3, EventImportImageOutcome.Downloaded, fromPage: true),
            Item(4, EventImportImageOutcome.Reused, fromPage: true),
            Item(5, EventImportImageOutcome.Reused),
            Item(6, EventImportImageOutcome.NoImage),
            Item(7, EventImportImageOutcome.Failed, reason: ImageFetchFailureReason.Expired),
            Item(8, EventImportImageOutcome.Failed, fromPage: true, reason: ImageFetchFailureReason.NoImageOnPage),
            Item(9, EventImportImageOutcome.RowNotImported, eventId: null));

        var report = EventImportReport.From(job);

        Assert.Equal((20, 18, 1, 1), (report.TotalRows, report.CreatedCount, report.SkippedExcludedCount, report.NotImportedCount));
        Assert.Equal((2, 2, 1, 1, 2, 0), (report.Downloaded, report.ExtractedFromPage, report.Reused, report.NoImage, report.Failed, report.Pending));
        Assert.Equal((7, 7), (report.ImagesToProcess, report.ImagesProcessed));
        Assert.Equal([7, 8], report.RowsWithoutImage.Select(i => i.RowNumber));
        Assert.Equal([9], report.RowsNotImported.Select(i => i.RowNumber));
    }

    [Fact]
    public void From_RowsNotReached_AreListedAsNotProcessedForRetryOrManualUpload()
    {
        var job = Job(EventImportJobStatus.Completed,
            Item(1, EventImportImageOutcome.Downloaded),
            Item(2, EventImportImageOutcome.Pending),
            Item(3, EventImportImageOutcome.Pending),
            Item(4, EventImportImageOutcome.Failed, reason: ImageFetchFailureReason.Timeout));

        var report = EventImportReport.From(job);

        Assert.Equal((2, 2, 4), (report.Pending, report.ImagesProcessed, report.ImagesToProcess));
        Assert.Equal([2, 3, 4], report.RowsWithoutImage.Select(i => i.RowNumber));
        Assert.Equal("Not processed", EventImportReport.GetResultLabel(report.RowsWithoutImage[0]));
    }

    [Fact]
    public void From_NothingToFetch_HasNoImageRows()
    {
        var report = EventImportReport.From(Job(EventImportJobStatus.Completed, Item(1, EventImportImageOutcome.NoImage)));

        Assert.Equal(0, report.ImagesToProcess);
        Assert.Empty(report.RowsWithoutImage);
    }

    [Fact]
    public void GetReasonLabel_EveryReasonHasALabel()
    {
        foreach (var reason in Enum.GetValues<ImageFetchFailureReason>())
        {
            Assert.False(string.IsNullOrWhiteSpace(EventImportReport.GetReasonLabel(reason)));
        }

        Assert.Equal("Forbidden address", EventImportReport.GetReasonLabel(ImageFetchFailureReason.ForbiddenAddress));
        Assert.Equal("", EventImportReport.GetReasonLabel(null));
    }

    [Fact]
    public void CsvWriter_WritesBomHeaderAndOneLinePerRowInRowOrder()
    {
        var failed = Item(2, EventImportImageOutcome.Failed, reason: ImageFetchFailureReason.Expired);
        failed.EventName = "Концерт, \"Джаз\"";
        failed.SourceImageUrl = "https://scontent.fbcdn.net/v/a.jpg?oh=1&oe=6A1B2C3D";
        failed.Detail = "HTTP 403.";
        failed.ProcessedAtUtc = new DateTime(2026, 11, 21, 9, 30, 0, DateTimeKind.Utc);
        var job = Job(EventImportJobStatus.Completed, failed, Item(1, EventImportImageOutcome.Downloaded, fromPage: true));

        var bytes = EventImportReportCsvWriter.Write(job);

        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes[..3]);
        var lines = Encoding.UTF8.GetString(bytes[3..]).TrimEnd().Split("\r\n");
        Assert.Equal("Row,Event ID,Event Name,Result,Failure Reason,Detail,Image URL,Ticket URL,Processed At (UTC)", lines[0]);
        Assert.StartsWith("1,1,Event 1,Extracted from page,", lines[1]);
        Assert.Equal(
            "2,1,\"Концерт, \"\"Джаз\"\"\",Failed,Expired address,HTTP 403.,https://scontent.fbcdn.net/v/a.jpg?oh=1&oe=6A1B2C3D,,2026-11-21 09:30:00",
            lines[2]);
        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void SofiaTimeZone_FromUtc_AppliesWinterAndSummerOffset()
    {
        Assert.Equal(new DateTime(2026, 11, 21, 11, 30, 0), SofiaTimeZone.FromUtc(new DateTime(2026, 11, 21, 9, 30, 0)));
        Assert.Equal(new DateTime(2026, 7, 1, 12, 30, 0), SofiaTimeZone.FromUtc(new DateTime(2026, 7, 1, 9, 30, 0)));
    }
}
