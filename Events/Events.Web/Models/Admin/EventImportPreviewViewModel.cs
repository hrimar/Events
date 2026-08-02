using Events.Models.Enums;
using Events.Services.Import.Models;

namespace Events.Web.Models.Admin;

public class EventImportPreviewViewModel
{
    public Guid BatchId { get; set; }
    public string OriginalFileName { get; set; } = "";
    public List<ImportRowResult> Rows { get; set; } = new();

    public List<CategoryOption> AvailableCategories { get; set; } = new();
    public List<SubCategoryOption> AvailableSubCategories { get; set; } = new();
    public List<TagOption> AvailableTags { get; set; } = new();
}

/// <summary>Posted-back shape from the Preview screen: the admin's corrections for every row.</summary>
public class EventImportPreviewEditViewModel
{
    public Guid BatchId { get; set; }
    public List<ImportRowEditViewModel> Rows { get; set; } = new();
}

public class ImportRowEditViewModel
{
    public int RowNumber { get; set; }
    public bool Excluded { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public EventStatus? Status { get; set; }
}
