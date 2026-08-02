namespace Events.Services.Import.Models;

/// <summary>
/// A single data row read from an uploaded import file, with column values keyed by header name.
/// </summary>
public class RawImportRow
{
    public int RowNumber { get; set; }

    public Dictionary<string, string?> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// The raw, format-agnostic result of parsing an uploaded .xlsx or .csv file, before any
/// business-level mapping (category/subcategory/tag resolution, validation) is applied.
/// </summary>
public class RawImportSheet
{
    public List<string> Headers { get; set; } = new();

    public List<RawImportRow> Rows { get; set; } = new();
}
