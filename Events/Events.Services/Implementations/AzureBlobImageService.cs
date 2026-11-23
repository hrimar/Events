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
    private readonly ImageVariantGenerator _variantGenerator;
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
        ImageVariantGenerator variantGenerator,
        IEventRepository eventRepository,
        ILogger<AzureBlobImageService> logger)
    {
        _containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
        _variantGenerator = variantGenerator ?? throw new ArgumentNullException(nameof(variantGenerator));
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

        // Same storage as the Excel import: original unchanged + 1200/480 WebP variants (GS-REQ-009, 9.4).
        // No minimum size here: the admin upload keeps its current rules (owner decision, 20.11.2026).
        MediaStoreResult result;
        try
        {
            await using var fileStream = file.OpenReadStream();
            using var buffer = new MemoryStream();
            await fileStream.CopyToAsync(buffer);

            result = await StoreEventImageAsync(buffer.ToArray(), requireMinimumSize: false);
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

    // Images stored by content (GS-REQ-009, 9.1.3, 9.1.4, 9.1.6, 9.4). The image id is the SHA-256 based id from
    // MediaImagePaths, so the same image always gets the same names and storing it again writes nothing
    // (acceptance criterion 4 of GS-REQ-009).
    public async Task<MediaStoreResult> StoreEventImageAsync(byte[] content, bool requireMinimumSize, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        var format = ImageFormatSniffer.Detect(content);
        if (format == null)
        {
            return MediaStoreResult.Failure(ImageFetchFailureReason.UnsupportedType, "Content is not JPEG, PNG, WebP or GIF.");
        }

        var imageId = MediaImagePaths.ComputeImageId(content);

        // The thumbnail is written last, so its presence means the whole set is complete.
        var thumbnailBlob = _containerClient.GetBlobClient(MediaImagePaths.ThumbnailBlobName(imageId));
        if ((await thumbnailBlob.ExistsAsync(cancellationToken)).Value)
        {
            return MediaStoreResult.Success(imageId, reused: true);
        }

        var variants = _variantGenerator.Generate(content, requireMinimumSize);
        if (!variants.IsSuccess)
        {
            return MediaStoreResult.Failure(variants.FailureReason!.Value, variants.Detail!);
        }

        try
        {
            // Original unchanged, as an archive (GS-REQ-009, 9.4).
            await UploadBlobAsync(
                MediaImagePaths.OriginalBlobName(imageId, format.Value), content, ImageFormatSniffer.GetContentType(format.Value), cancellationToken);
            await UploadBlobAsync(MediaImagePaths.LargeBlobName(imageId), variants.Large, "image/webp", cancellationToken);
            await UploadBlobAsync(MediaImagePaths.ThumbnailBlobName(imageId), variants.Thumbnail, "image/webp", cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to write image {ImageId} to storage", imageId);
            return MediaStoreResult.Failure(ImageFetchFailureReason.ProcessingError, "Could not write the image to storage.");
        }

        _logger.LogInformation("Stored image {ImageId} ({Format}, {Width}x{Height}px)", imageId, format, variants.Width, variants.Height);
        return MediaStoreResult.Success(imageId, reused: false);
    }

    private async Task UploadBlobAsync(string blobName, byte[] content, string contentType, CancellationToken cancellationToken)
    {
        // Content type is stored on the blob, so MediaController returns image/webp, image/png etc.
        var options = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } };
        await _containerClient.GetBlobClient(blobName).UploadAsync(new BinaryData(content), options, cancellationToken);
    }

    // Deletes the whole set of an image stored by content. The caller checks first that no other event uses it.
    private async Task DeleteStoredImageSetAsync(string imageId)
    {
        // Thumbnail first: without it StoreEventImageAsync no longer treats the set as complete, so a half-deleted
        // set is written again in full by the next upload of the same content.
        await _containerClient.DeleteBlobIfExistsAsync(MediaImagePaths.ThumbnailBlobName(imageId));
        await _containerClient.DeleteBlobIfExistsAsync(MediaImagePaths.LargeBlobName(imageId));

        // The original keeps its own extension: one call per accepted format instead of listing the container.
        foreach (var format in Enum.GetValues<DetectedImageFormat>())
        {
            await _containerClient.DeleteBlobIfExistsAsync(MediaImagePaths.OriginalBlobName(imageId, format));
        }

        _logger.LogInformation("Deleted image {ImageId}", imageId);
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

            await DeleteStoredImageSetAsync(imageId);
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
