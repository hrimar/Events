using Microsoft.AspNetCore.Http;

namespace Events.Services.Interfaces;

/// <summary>
/// Service for handling image uploads to cloud storage.
/// Handles image validation, resizing, and URL generation.
/// </summary>
public interface IImageUploadService
{
    /// <summary>
    /// Uploads an image file and returns original and thumbnail URLs.
    /// Performs synchronous upload with automatic resizing.
    /// Supports both numeric event IDs and GUID session IDs.
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="eventIdOrSessionId">Event ID (int) or session GUID (string)</param>
    /// <returns>Tuple containing (originalImageUrl, thumbnailUrl)</returns>
    /// <exception cref="ArgumentException">Thrown when file validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when upload fails</exception>
    Task<(string OriginalUrl, string ThumbnailUrl)> UploadEventImageAsync(IFormFile file, string eventIdOrSessionId);

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
