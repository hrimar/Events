using Events.Services.Interfaces;
using Events.Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers.Api;

/// <summary>
/// API controller for managing event images.
/// Handles image upload to Azure Blob Storage with automatic resizing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
public class EventImagesController : ControllerBase
{
    private readonly IImageUploadService _imageUploadService;
    private readonly ILogger<EventImagesController> _logger;

    public EventImagesController(IImageUploadService imageUploadService, ILogger<EventImagesController> logger)
    {
        _imageUploadService = imageUploadService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads an image file for an event.
    /// Returns both original and thumbnail image URLs along with metadata.
    /// Supports both new events (with event ID) and temporary uploads (generates GUID).
    /// </summary>
    /// <param name="file">Image file to upload (JPEG, PNG, WebP)</param>
    /// <param name="eventId">Event ID (optional) - if not provided, generates GUID</param>
    /// <returns>URLs of uploaded original and thumbnail images with metadata</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ImageUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImageUploadResponseDto>> UploadEventImage([FromForm] IFormFile file, [FromForm] string eventId = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file provided");
                return BadRequest(new { message = "No file provided" });
            }

            // Use provided event ID or generate a temporary session ID
            var uploadId = string.IsNullOrWhiteSpace(eventId) ? Guid.NewGuid().ToString() : eventId;

            _logger.LogInformation("Image upload started for ID: {UploadId}, File: {FileName}, Size: {FileSize} bytes",
                uploadId, file.FileName, file.Length);

            // Validate file
            var validation = _imageUploadService.ValidateImageFile(file);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Image validation failed for {UploadId}: {ErrorMessage}", uploadId, validation.ErrorMessage);
                return BadRequest(new { message = validation.ErrorMessage });
            }

            var uploadStartTime = DateTime.UtcNow;

            // Upload image
            var (originalUrl, thumbnailUrl) = await _imageUploadService.UploadEventImageAsync(file, uploadId);

            var uploadEndTime = DateTime.UtcNow;
            var uploadDuration = uploadEndTime - uploadStartTime;

            _logger.LogInformation("Image uploaded successfully for {UploadId}. Duration: {DurationMs}ms, Original: {OriginalUrl}",
                uploadId, uploadDuration.TotalMilliseconds, originalUrl);

            return Ok(new ImageUploadResponseDto
            {
                OriginalImageUrl = originalUrl,
                ThumbnailImageUrl = thumbnailUrl,
                FileSizeBytes = file.Length,
                UploadedAtUtc = DateTime.UtcNow,
                UploadDurationMs = (long)uploadDuration.TotalMilliseconds,
                Message = "Image uploaded successfully"
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Argument validation error during image upload");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation during image upload");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during image upload: {ExceptionType}", ex.GetType().Name);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error uploading image" });
        }
    }
}
