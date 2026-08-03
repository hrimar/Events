using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Events.Services.Import.Models;
using Events.Services.Interfaces;

namespace Events.Services.Import.Parsers;

/// <summary>
/// Reads a .csv file into a <see cref="RawImportSheet"/>. Tolerant of missing/extra fields
/// and malformed data so a single bad cell doesn't abort the whole import.
/// </summary>
public class CsvEventImportParser : IEventImportFileParser
{
    public bool CanParse(string fileName) => fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);

    public Task<RawImportSheet> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            DetectDelimiter = true,
        };

        using var reader = new StreamReader(fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        using var csv = new CsvReader(reader, config);

        var sheet = new RawImportSheet();

        if (!csv.Read() || !csv.ReadHeader() || csv.HeaderRecord == null)
        {
            return Task.FromResult(sheet);
        }

        var headers = csv.HeaderRecord.Select(h => h.Trim()).ToList();
        sheet.Headers = headers;

        while (csv.Read())
        {
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var hasAnyValue = false;

            foreach (var header in headers)
            {
                var value = csv.GetField(header)?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    hasAnyValue = true;
                }

                values[header] = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            if (!hasAnyValue)
            {
                continue;
            }

            // csv.Parser.Row is the actual 1-based line number in the file (header included),
            // matching the row numbering convention used by the .xlsx parser.
            sheet.Rows.Add(new RawImportRow { RowNumber = csv.Parser.Row, Values = values });
        }

        return Task.FromResult(sheet);
    }
}
