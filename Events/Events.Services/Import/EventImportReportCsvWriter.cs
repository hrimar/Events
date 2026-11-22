using System.Globalization;
using System.Text;
using CsvHelper;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Services.Import;

// The import report as CSV, one line per row of the file (GS-REQ-009, 9.3.3). Comma-separated like the import
// template, UTF-8 with BOM so Excel shows Cyrillic event names correctly.
public static class EventImportReportCsvWriter
{
    public static byte[] Write(EventImportJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        var jobCompleted = job.Status == EventImportJobStatus.Completed;

        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            foreach (var header in new[]
                     {
                         "Row", "Event ID", "Event Name", "Result", "Failure Reason", "Detail", "Image URL", "Ticket URL", "Processed At (UTC)"
                     })
            {
                csv.WriteField(header);
            }

            csv.NextRecord();

            foreach (var item in job.Items.OrderBy(i => i.RowNumber))
            {
                csv.WriteField(item.RowNumber);
                csv.WriteField(item.EventId?.ToString(CultureInfo.InvariantCulture) ?? "");
                csv.WriteField(item.EventName);
                csv.WriteField(EventImportReport.GetResultLabel(item, jobCompleted));
                csv.WriteField(EventImportReport.GetReasonLabel(item.FailureReason));
                csv.WriteField(item.Detail ?? "");
                csv.WriteField(item.SourceImageUrl ?? "");
                csv.WriteField(item.PageUrl ?? "");
                csv.WriteField(item.ProcessedAtUtc?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? "");
                csv.NextRecord();
            }
        }

        return stream.ToArray();
    }
}
