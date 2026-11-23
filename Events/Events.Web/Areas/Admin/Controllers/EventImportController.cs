using Events.Data.Repositories.Interfaces;
using Events.Services.Import;
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
    private const int RecentJobsCount = 10;

    // Images are copied inside the Confirm request, so one file is limited to a size that always finishes in time
    // (owner decision, 23.11.2026: usually 20-30 events, at most 50; GS-REQ-009, 9.3.4 replaced).
    public const int MaxRowsPerImport = 50;

    private readonly IEventImportService _eventImportService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ITagService _tagService;
    private readonly IEventImportJobRepository _jobRepository;
    private readonly EventImportImageJobRunner _imageJobRunner;
    private readonly EventImportBatchCache _batchCache;
    private readonly ILogger<EventImportController> _logger;

    public EventImportController(
        IEventImportService eventImportService,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ITagService tagService,
        IEventImportJobRepository jobRepository,
        EventImportImageJobRunner imageJobRunner,
        EventImportBatchCache batchCache,
        ILogger<EventImportController> logger)
    {
        _eventImportService = eventImportService;
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _tagService = tagService;
        _jobRepository = jobRepository;
        _imageJobRunner = imageJobRunner;
        _batchCache = batchCache;
        _logger = logger;
    }

    // GET: Admin/EventImport/Upload
    public async Task<IActionResult> Upload()
    {
        return View(new EventImportUploadViewModel { RecentJobs = await _jobRepository.GetRecentAsync(RecentJobsCount) });
    }

    // GET: Admin/EventImport/Job/5 - import report with the image results (GS-REQ-009, 9.3.3, 9.3.5).
    public async Task<IActionResult> Job(int id)
    {
        var job = await _jobRepository.GetWithItemsAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        return View(new EventImportJobViewModel { Job = job, Report = EventImportReport.From(job) });
    }

    // GET: Admin/EventImport/JobCsv/5 - the same report as CSV (GS-REQ-009, 9.3.3).
    public async Task<IActionResult> JobCsv(int id)
    {
        var job = await _jobRepository.GetWithItemsAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        var fileName = $"import-report-{job.Id}-{job.CreatedAtUtc:yyyyMMdd-HHmm}.csv";
        return File(EventImportReportCsvWriter.Write(job), "text/csv", fileName);
    }

    // POST: Admin/EventImport/RetryRemaining/5 - copies the images of the rows the first run did not reach
    // (time limit, app restart); failed rows are fixed by uploading an image by hand (GS-REQ-009, 9.3.5).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RetryRemaining(int id)
    {
        // Not bound to the request: closing the tab does not stop the copy halfway.
        await _imageJobRunner.RunAsync(id, CancellationToken.None);
        return RedirectToAction(nameof(Job), new { id });
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

            if (batch.Rows.Count > MaxRowsPerImport)
            {
                return View(new EventImportUploadViewModel
                {
                    ErrorMessage = $"The file has {batch.Rows.Count} rows. Import at most {MaxRowsPerImport} events at once - split the file."
                });
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

    // POST: Admin/EventImport/Confirm — applies any last-second corrections from the Preview form,
    // then commits the accepted rows to the database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(EventImportPreviewEditViewModel model)
    {
        var batch = _batchCache.Get(GetUserKey(), model.BatchId);
        if (batch == null)
        {
            TempData["ErrorMessage"] = "This import session has expired. Please upload the file again.";
            return RedirectToAction(nameof(Upload));
        }

        ApplyEdits(batch, model);

        if (batch.Rows.Any(r => !r.Excluded && r.Severity == ImportRowSeverity.Error))
        {
            _batchCache.Store(GetUserKey(), batch);
            TempData["ErrorMessage"] = "Some rows still have unresolved errors. Exclude them or fix the source file before confirming.";
            return RedirectToAction(nameof(Preview), new { batchId = batch.BatchId });
        }

        var result = await _eventImportService.CommitAsync(batch, GetUserKey());
        _batchCache.Remove(GetUserKey(), batch.BatchId);

        _logger.LogInformation(
            "Event import batch {BatchId} committed: {Created} created, {Skipped} skipped, {Failed} failed",
            batch.BatchId, result.CreatedCount, result.SkippedExcludedCount, result.FailedCount);

        // Images are copied now, while the Facebook addresses are still valid (GS-REQ-009, 9.1), then the report shows
        // the result (9.3.3). Not bound to the request: closing the tab does not stop the copy halfway.
        // The old result page stays as a fallback if the image job could not be saved.
        if (result.JobId != null)
        {
            await _imageJobRunner.RunAsync(result.JobId.Value, CancellationToken.None);
            return RedirectToAction(nameof(Job), new { id = result.JobId.Value });
        }

        return View("Result", result);
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
                .Where(c => c.Id != 11) // Exclude Undefined category (Category.Id, not (int)EventCategory.Undefined)
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
