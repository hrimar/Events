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
    private readonly EventImportBatchCache _batchCache;
    private readonly ILogger<EventImportController> _logger;

    public EventImportController(
        IEventImportService eventImportService,
        EventImportBatchCache batchCache,
        ILogger<EventImportController> logger)
    {
        _eventImportService = eventImportService;
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
    public IActionResult Preview(Guid batchId)
    {
        var batch = _batchCache.Get(GetUserKey(), batchId);
        if (batch == null)
        {
            TempData["ErrorMessage"] = "This import session has expired. Please upload the file again.";
            return RedirectToAction(nameof(Upload));
        }

        return View(batch);
    }

    private string GetUserKey() => User.Identity?.Name ?? "unknown";
}
