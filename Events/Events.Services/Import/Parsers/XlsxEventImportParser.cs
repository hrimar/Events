using ClosedXML.Excel;
using Events.Services.Import.Models;
using Events.Services.Interfaces;

namespace Events.Services.Import.Parsers;

/// <summary>
/// Reads an .xlsx workbook's first worksheet into a <see cref="RawImportSheet"/>.
/// The first non-empty row is treated as the header row; fully blank rows are skipped.
/// </summary>
public class XlsxEventImportParser : IEventImportFileParser
{
    public bool CanParse(string fileName) => fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);

    public Task<RawImportSheet> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var usedRange = worksheet.RangeUsed();

        var sheet = new RawImportSheet();
        if (usedRange == null)
        {
            return Task.FromResult(sheet);
        }

        var rows = usedRange.RowsUsed().ToList();
        if (rows.Count == 0)
        {
            return Task.FromResult(sheet);
        }

        var headerRow = rows[0];
        var headers = headerRow.Cells().Select(c => c.GetString().Trim()).ToList();
        sheet.Headers = headers;

        foreach (var row in rows.Skip(1))
        {
            var cells = row.Cells(1, headers.Count).Select(c => c.GetString().Trim()).ToList();
            if (cells.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < headers.Count; i++)
            {
                var value = i < cells.Count ? cells[i] : null;
                values[headers[i]] = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            // Use the worksheet's actual row number so the admin can locate the row in Excel.
            sheet.Rows.Add(new RawImportRow { RowNumber = row.RowNumber(), Values = values });
        }

        return Task.FromResult(sheet);
    }
}
