namespace Events.Web.Models.DTOs;

/// <summary>
/// DTO for image upload response containing URLs and metadata.
/// </summary>
public class ImageUploadResponseDto
{
    /// <summary>
    /// Full URL to the original uploaded image in Blob Storage.
    /// </summary>
    public string OriginalImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Full URL to the thumbnail image in Blob Storage (400x300px, cropped).
    /// </summary>
    public string ThumbnailImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Size of uploaded file in bytes.
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// UTC timestamp when image was uploaded.
    /// </summary>
    public DateTime UploadedAtUtc { get; set; }

    /// <summary>
    /// Duration of upload operation in milliseconds.
    /// </summary>
    public long UploadDurationMs { get; set; }

    /// <summary>
    /// Status message describing the upload result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
