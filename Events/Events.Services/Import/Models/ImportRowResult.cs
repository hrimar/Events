using Events.Models.Enums;

namespace Events.Services.Import.Models;

public enum ImportRowSeverity
{
    Ok,
    Warning,
    Error
}

/// <summary>
/// The outcome of mapping and validating a single row from an uploaded import file: the typed
/// fields resolved so far, plus enough raw text and diagnostics for an admin to correct anything
/// that didn't resolve automatically before the row is committed to the database.
/// </summary>
public class ImportRowResult
{
    public int RowNumber { get; set; }

    public ImportRowSeverity Severity { get; set; } = ImportRowSeverity.Ok;

    public List<string> Messages { get; set; } = new();

    /// <summary>Set by the admin in the preview screen to leave this row out of the final commit.</summary>
    public bool Excluded { get; set; }

    public bool IsDuplicate { get; set; }
    public int? DuplicateEventId { get; set; }
    public int? DuplicateOfRowNumber { get; set; }

    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public string? City { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }

    public string RawCategoryText { get; set; } = "";
    public int? CategoryId { get; set; }

    public string RawSubCategoryText { get; set; } = "";
    public int? SubCategoryId { get; set; }

    public string RawTagsText { get; set; } = "";
    public List<int> MatchedTagIds { get; set; } = new();
    public List<string> UnmatchedTagNames { get; set; } = new();

    public string RawFreeEventText { get; set; } = "";
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }

    public string RawStatusText { get; set; } = "";
    public EventStatus? Status { get; set; }

    public bool IsFeatured { get; set; }

    public int? CanonicalVenueId { get; set; }

    public void AddError(string message)
    {
        Messages.Add(message);
        Severity = ImportRowSeverity.Error;
    }

    public void AddWarning(string message)
    {
        Messages.Add(message);
        if (Severity != ImportRowSeverity.Error)
        {
            Severity = ImportRowSeverity.Warning;
        }
    }
}
