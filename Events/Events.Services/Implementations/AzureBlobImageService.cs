using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Mime;
using Events.Data.Repositories.Interfaces;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Media;

namespace Events.Services.Implementations;

/// <summary>
/// Implementation of IImageUploadService using Azure Blob Storage.
/// Handles synchronous image upload with automatic resizing.
/// </summary>
public class AzureBlobImageService : IImageUploadService
{
    private readonly BlobContainerClient _containerClient;
    private readonly IMediaImageStore _mediaImageStore;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<AzureBlobImageService> _logger;

    // Image constraints
    private const int MaxFileSizeMB = 5;
    private const int MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
    private const int OriginalMaxWidth = 1200;
    private const int OriginalMaxHeight = 900;

    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

    public AzureBlobImageService(
        BlobContainerClient containerClient,
        IMediaImageStore mediaImageStore,
        IEventRepository eventRepository,
        ILogger<AzureBlobImageService> logger)
    {
        _containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
        _mediaImageStore = mediaImageStore ?? throw new ArgumentNullException(nameof(mediaImageStore));
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValidationResult ValidateImageFile(IFormFile file)
    {
        if (file == null)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "No file provided",
                MaxFileSizeBytes = MaxFileSizeBytes,
                AllowedMimeTypes = AllowedMimeTypes
            };
        }

        // Check MIME type
        if (!AllowedMimeTypes.Contains(file.ContentType?.ToLowerInvariant()))
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Invalid file format. Allowed formats: {string.Join(", ", AllowedMimeTypes)}",
                MaxFileSizeBytes = MaxFileSizeBytes,
                AllowedMimeTypes = AllowedMimeTypes
            };
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = $"File is too large. Maximum size: {MaxFileSizeMB}MB",
                MaxFileSizeBytes = MaxFileSizeBytes,
                AllowedMimeTypes = AllowedMimeTypes
            };
        }

        // Check if file is a valid image
        try
        {
            using var stream = file.OpenReadStream();
            using var image = Image.Load(stream);
            // Image loaded successfully
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid image file: {FileName}", file.FileName);
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "File is not a valid image",
                MaxFileSizeBytes = MaxFileSizeBytes,
                AllowedMimeTypes = AllowedMimeTypes
            };
        }

        return new ValidationResult
        {
            IsValid = true,
            MaxFileSizeBytes = MaxFileSizeBytes,
            AllowedMimeTypes = AllowedMimeTypes
        };
    }

    public async Task<(string OriginalUrl, string ThumbnailUrl)> UploadEventImageAsync(IFormFile file, string eventIdOrSessionId)
    {
        var validation = ValidateImageFile(file);
        if (!validation.IsValid)
        {
            throw new ArgumentException(validation.ErrorMessage);
        }

        // Same store as the Excel import: original unchanged + 1200/480 WebP variants (GS-REQ-009, 9.4).
        // No minimum size here: the admin upload keeps its current rules (owner decision, 20.11.2026).
        MediaStoreResult result;
        try
        {
            await using var fileStream = file.OpenReadStream();
            using var buffer = new MemoryStream();
            await fileStream.CopyToAsync(buffer);

            result = await _mediaImageStore.StoreAsync(buffer.ToArray(), requireMinimumSize: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for event {EventId}", eventIdOrSessionId);
            throw new InvalidOperationException("Failed to upload image to storage", ex);
        }

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Image for event {EventId} was not stored: {Reason} ({Detail})",
                eventIdOrSessionId, result.FailureReason, result.Detail);

            if (result.FailureReason == ImageFetchFailureReason.ProcessingError)
            {
                throw new InvalidOperationException("Failed to upload image to storage");
            }

            throw new ArgumentException(result.Detail);
        }

        _logger.LogInformation("Image for event {EventId} stored as {ImageId} (reused: {Reused})",
            eventIdOrSessionId, result.ImageId, result.Reused);

        return (result.LargeUrl!, result.ThumbnailUrl!);
    }

    public async Task DeleteEventImagesAsync(int eventId, string? imageUrl, string? thumbnailUrl)
    {
        var imageId = MediaImagePaths.TryGetImageId(imageUrl) ?? MediaImagePaths.TryGetImageId(thumbnailUrl);
        if (imageId == null)
        {
            // Older uploads (original/{id}_{ticks}.jpg + thumbnails/...): one event per file, deleted as before.
            await DeleteLegacyImageAsync(eventId, imageUrl);
            await DeleteLegacyImageAsync(eventId, thumbnailUrl);
            return;
        }

        try
        {
            var largeUrl = MediaImagePaths.ToUrl(MediaImagePaths.LargeBlobName(imageId));
            var smallUrl = MediaImagePaths.ToUrl(MediaImagePaths.ThumbnailBlobName(imageId));

            var otherEvents = await _eventRepository.CountOtherEventsUsingImageAsync(largeUrl, smallUrl, eventId);
            if (otherEvents > 0)
            {
                _logger.LogInformation("Kept image {ImageId} of deleted event {EventId}: used by {Count} other event(s)",
                    imageId, eventId, otherEvents);
                return;
            }

            await _mediaImageStore.DeleteAsync(imageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImageId} of event {EventId}", imageId, eventId);
        }
    }

    private async Task DeleteLegacyImageAsync(int eventId, string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return;
        }

        var deleted = await DeleteImageAsync(imageUrl);
        if (deleted)
        {
            _logger.LogInformation("Deleted image for event {EventId}: {ImageUrl}", eventId, imageUrl);
        }
        else
        {
            _logger.LogWarning("Failed to delete image for event {EventId}: {ImageUrl}", eventId, imageUrl);
        }
    }

    public async Task<string> UploadVenueImageAsync(IFormFile file, string venueIdOrSessionId)
    {
        var validation = ValidateImageFile(file);
        if (!validation.IsValid)
        {
            throw new ArgumentException(validation.ErrorMessage);
        }

        try
        {
            var timestamp = DateTime.UtcNow.Ticks;
            var originalFileName = $"venues/original/{venueIdOrSessionId}_{timestamp}.jpg";

            var originalUrl = await UploadResizedOriginalAsync(file, originalFileName);

            _logger.LogInformation("Original image uploaded for venue {VenueId}: {BlobName}", venueIdOrSessionId, originalFileName);

            return originalUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for venue {VenueId}", venueIdOrSessionId);
            throw new InvalidOperationException("Failed to upload image to storage", ex);
        }
    }

    // Resizes (if larger than the max dimensions) and uploads an image as the "original" variant, returning its full URL.
    private async Task<string> UploadResizedOriginalAsync(IFormFile file, string blobFileName)
    {
        await using var fileStream = file.OpenReadStream();
        using var originalImage = Image.Load(fileStream);

        if (originalImage.Width > OriginalMaxWidth || originalImage.Height > OriginalMaxHeight)
        {
            originalImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(OriginalMaxWidth, OriginalMaxHeight),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Lanczos3
            }));
        }

        await using var originalStream = new MemoryStream();
        originalImage.SaveAsJpeg(originalStream);
        originalStream.Position = 0;

        var originalBlobClient = _containerClient.GetBlobClient(blobFileName);
        await originalBlobClient.UploadAsync(originalStream, overwrite: true);

        // Relative URL, served through MediaController so the public address stays under our own domain.
        return "/media/" + blobFileName;
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return false;
        }

        try
        {
            // Get blob name from URL
            var blobName = ExtractBlobNameFromUrl(imageUrl);
            if (string.IsNullOrEmpty(blobName))
            {
                _logger.LogWarning("Could not extract blob name from URL: {ImageUrl}", imageUrl);
                return false;
            }

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation("Image deleted: {BlobName}", blobName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
            return false;
        }
    }

    private static string? ExtractBlobNameFromUrl(string imageUrl)
    {
        // Current format: relative URL served through MediaController, e.g. "/media/original/xxx.jpg".
        const string mediaPrefix = "/media/";
        if (imageUrl.StartsWith(mediaPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl[mediaPrefix.Length..];
        }

        // Legacy format: absolute blob storage URL, e.g. "https://...blob.core.windows.net/event-images/original/xxx.jpg".
        try
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return string.Join("/", parts.Skip(1));
            }
        }
        catch (Exception)
        {
            // Invalid URL
        }

        return null;
    }
}
