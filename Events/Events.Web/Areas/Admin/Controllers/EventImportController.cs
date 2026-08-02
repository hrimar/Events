using Events.Data.Repositories.Interfaces;
using Events.Services.Import.Models;
using Events.Services.Interfaces;
using Events.Web.Areas.Admin.Services;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class EventImportController : Controller
{
    private static readonly string[] AllowedExtensions = { ".xlsx", ".csv" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private readonly IEventImportService _eventImportService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ITagService _tagService;
    private readonly EventImportBatchCache _batchCache;
    private readonly ILogger<EventImportController> _logger;

    public EventImportController(
        IEventImportService eventImportService,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ITagService tagService,
        EventImportBatchCache batchCache,
        ILogger<EventImportController> logger)
    {
        _eventImportService = eventImportService;
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _tagService = tagService;
        _batchCache = batchCache;
        _logger = logger;
    }

    // GET: Admin/EventImport/Upload
    public IActionResult Upload()
    {
        return View(new EventImportUploadViewModel());
    }

    // POST: Admin/EventImport/Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return View(new EventImportUploadViewModel { ErrorMessage = "Please select a file to upload." });
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return View(new EventImportUploadViewModel { ErrorMessage = "Only .xlsx and .csv files are supported." });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return View(new EventImportUploadViewModel { ErrorMessage = "File is too large. Maximum allowed size is 10 MB." });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var batch = await _eventImportService.ParseAndValidateAsync(stream, file.FileName);

            if (batch.Rows.Count == 0)
            {
                return View(new EventImportUploadViewModel { ErrorMessage = "The file contains no data rows." });
            }

            _batchCache.Store(GetUserKey(), batch);

            return RedirectToAction(nameof(Preview), new { batchId = batch.BatchId });
        }
        catch (NotSupportedException ex)
        {
            return View(new EventImportUploadViewModel { ErrorMessage = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing import file {FileName}", file.FileName);
            return View(new EventImportUploadViewModel { ErrorMessage = "An error occurred while reading the file. Please check its format and try again." });
        }
    }

    // GET: Admin/EventImport/Preview/{batchId}
    public async Task<IActionResult> Preview(Guid batchId)
    {
        var batch = _batchCache.Get(GetUserKey(), batchId);
        if (batch == null)
        {
            TempData["ErrorMessage"] = "This import session has expired. Please upload the file again.";
            return RedirectToAction(nameof(Upload));
        }

        return View(await BuildPreviewViewModelAsync(batch));
    }

    // POST: Admin/EventImport/Preview — applies the admin's corrections and re-displays the preview.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Preview(EventImportPreviewEditViewModel model)
    {
        var batch = _batchCache.Get(GetUserKey(), model.BatchId);
        if (batch == null)
        {
            TempData["ErrorMessage"] = "This import session has expired. Please upload the file again.";
            return RedirectToAction(nameof(Upload));
        }

        ApplyEdits(batch, model);
        _batchCache.Store(GetUserKey(), batch);

        return View(await BuildPreviewViewModelAsync(batch));
    }

    private static void ApplyEdits(EventImportBatch batch, EventImportPreviewEditViewModel model)
    {
        var editsByRow = model.Rows.ToDictionary(r => r.RowNumber);

        foreach (var row in batch.Rows)
        {
            if (!editsByRow.TryGetValue(row.RowNumber, out var edit))
            {
                continue;
            }

            row.Excluded = edit.Excluded;
            row.CategoryId = edit.CategoryId;
            row.SubCategoryId = edit.SubCategoryId;
            row.MatchedTagIds = edit.TagIds;
            row.UnmatchedTagNames.Clear();
            row.Status = edit.Status;

            RecomputeSeverity(row);
        }
    }

    // Re-evaluates a row's severity after the admin corrects it: rows missing a field that this
    // screen can't edit (Name/Date/City/Location — parsed straight from the file) stay Error,
    // since they need the source file fixed and re-uploaded, not a dropdown pick here.
    private static void RecomputeSeverity(ImportRowResult row)
    {
        var hasUnfixableRequiredFieldMissing = string.IsNullOrWhiteSpace(row.Name)
            || row.Date == null
            || string.IsNullOrWhiteSpace(row.City)
            || string.IsNullOrWhiteSpace(row.Location);

        if (hasUnfixableRequiredFieldMissing)
        {
            row.Severity = ImportRowSeverity.Error;
            return;
        }

        var hasUnresolvedField = row.CategoryId == null
            || row.Status == null
            || row.IsDuplicate
            || (!string.IsNullOrWhiteSpace(row.RawSubCategoryText) && row.SubCategoryId == null);

        row.Severity = hasUnresolvedField ? ImportRowSeverity.Warning : ImportRowSeverity.Ok;
    }

    private async Task<EventImportPreviewViewModel> BuildPreviewViewModelAsync(EventImportBatch batch)
    {
        var categories = await _categoryRepository.GetAllAsync();
        var subCategories = await _subCategoryRepository.GetAllAsync();
        var tags = await _tagService.GetAllTagsAsync();

        return new EventImportPreviewViewModel
        {
            BatchId = batch.BatchId,
            OriginalFileName = batch.OriginalFileName,
            Rows = batch.Rows,
            AvailableCategories = categories
                .Where(c => c.Id != 11) // Exclude Undefined category
                .Select(c => new CategoryOption { Id = c.Id, Name = c.Name })
                .OrderBy(c => c.Name)
                .ToList(),
            AvailableSubCategories = subCategories
                .Select(sc => new SubCategoryOption { Id = sc.Id, Name = sc.Name, CategoryId = sc.CategoryId })
                .OrderBy(sc => sc.Name)
                .ToList(),
            AvailableTags = tags
                .Select(t => new TagOption { Id = t.Id, Name = t.Name })
                .OrderBy(t => t.Name)
                .ToList()
        };
    }

    private string GetUserKey() => User.Identity?.Name ?? "unknown";
}
