using Microsoft.AspNetCore.Http;

namespace Events.Services.Interfaces;

/// <summary>
/// Service for handling image uploads to cloud storage.
/// Handles image validation, resizing, and URL generation.
/// </summary>
public interface IImageUploadService
{
    /// <summary>
    /// Uploads an event image: keeps the original unchanged and stores 1200px and 480px WebP variants,
    /// named by content so the same image is stored once (GS-REQ-009, 9.4).
    /// Supports both numeric event IDs and GUID session IDs (used for logging only).
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="eventIdOrSessionId">Event ID (int) or session GUID (string)</param>
    /// <returns>Tuple containing (1200px image URL for Event.ImageUrl, 480px URL for Event.ThumbnailUrl)</returns>
    /// <exception cref="ArgumentException">Thrown when file validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when upload fails</exception>
    Task<(string OriginalUrl, string ThumbnailUrl)> UploadEventImageAsync(IFormFile file, string eventIdOrSessionId);

    /// <summary>
    /// Deletes the images of an event that is being deleted. Images stored by content (/media/1200|480/{id}.webp)
    /// are deleted only when no other event uses them (GS-REQ-009, 9.1.6); older uploads are deleted as before.
    /// Never throws: failures are logged.
    /// </summary>
    Task DeleteEventImagesAsync(int eventId, string? imageUrl, string? thumbnailUrl);

    /// <summary>
    /// Uploads an image file for a venue and returns the original image URL.
    /// Performs synchronous upload with automatic resizing. No thumbnail is generated,
    /// since venue pages only display the original photo.
    /// Supports both numeric venue IDs and GUID session IDs.
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="venueIdOrSessionId">Venue ID (int) or session GUID (string)</param>
    /// <returns>Full URL of the uploaded image</returns>
    /// <exception cref="ArgumentException">Thrown when file validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when upload fails</exception>
    Task<string> UploadVenueImageAsync(IFormFile file, string venueIdOrSessionId);

    /// <summary>
    /// Deletes an image from cloud storage.
    /// </summary>
    /// <param name="imageUrl">The full URL or blob name of the image to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteImageAsync(string imageUrl);

    /// <summary>
    /// Validates if a file is suitable for upload.
    /// Checks format, size, and other constraints.
    /// </summary>
    /// <param name="file">The file to validate</param>
    /// <returns>Validation result with error messages if invalid</returns>
    ValidationResult ValidateImageFile(IFormFile file);
}

/// <summary>
/// Result of image file validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates whether the file passed validation.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error message if validation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Maximum allowed file size in bytes.
    /// </summary>
    public long MaxFileSizeBytes { get; set; }

    /// <summary>
    /// Allowed MIME types for upload.
    /// </summary>
    public string[] AllowedMimeTypes { get; set; } = Array.Empty<string>();
}
