using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Mime;
using Events.Services.Interfaces;

namespace Events.Services.Implementations;

/// <summary>
/// Implementation of IImageUploadService using Azure Blob Storage.
/// Handles synchronous image upload with automatic resizing.
/// </summary>
public class AzureBlobImageService : IImageUploadService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<AzureBlobImageService> _logger;

    // Image constraints
    private const int MaxFileSizeMB = 5;
    private const int MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
    private const int ThumbnailWidth = 400;
    private const int ThumbnailHeight = 300;
    private const int OriginalMaxWidth = 1200;
    private const int OriginalMaxHeight = 900;

    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

    public AzureBlobImageService(BlobContainerClient containerClient, ILogger<AzureBlobImageService> logger)
    {
        _containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
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

        try
        {
            var timestamp = DateTime.UtcNow.Ticks;
            var originalFileName = $"original/{eventIdOrSessionId}_{timestamp}.jpg";
            var thumbnailFileName = $"thumbnails/{eventIdOrSessionId}_{timestamp}.jpg";

            // Save the original image
            await using (var fileStream = file.OpenReadStream())
            {
                using var originalImage = Image.Load(fileStream);

                // Resize the original image if it's larger
                if (originalImage.Width > OriginalMaxWidth || originalImage.Height > OriginalMaxHeight)
                {
                    originalImage.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(OriginalMaxWidth, OriginalMaxHeight),
                        Mode = ResizeMode.Max,
                        Sampler = KnownResamplers.Lanczos3
                    }));
                }

                // Upload the original image
                await using var originalStream = new MemoryStream();
                originalImage.SaveAsJpeg(originalStream);
                originalStream.Position = 0;

                var originalBlobClient = _containerClient.GetBlobClient(originalFileName);
                await originalBlobClient.UploadAsync(originalStream, overwrite: true);

                _logger.LogInformation("Original image uploaded for event {EventId}: {BlobName}", eventIdOrSessionId, originalFileName);
            }

            // Create and upload thumbnail
            var thumbnailUrl = await CreateAndUploadThumbnailAsync(file, thumbnailFileName, eventIdOrSessionId);

            var originalUrl = _containerClient.Uri.AbsoluteUri.TrimEnd('/') + "/" + originalFileName;

            return (originalUrl, thumbnailUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for event {EventId}", eventIdOrSessionId);
            throw new InvalidOperationException("Failed to upload image to storage", ex);
        }
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

    private async Task<string> CreateAndUploadThumbnailAsync(IFormFile file, string thumbnailFileName, string eventIdOrSessionId)
    {
        await using var fileStream = file.OpenReadStream();
        using var image = Image.Load(fileStream);

        // Resize the thumbnail
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(ThumbnailWidth, ThumbnailHeight),
            Mode = ResizeMode.Crop,
            Sampler = KnownResamplers.Lanczos3
        }));

        // Upload the thumbnail
        await using var thumbnailStream = new MemoryStream();
        image.SaveAsJpeg(thumbnailStream);
        thumbnailStream.Position = 0;

        var thumbnailBlobClient = _containerClient.GetBlobClient(thumbnailFileName);
        await thumbnailBlobClient.UploadAsync(thumbnailStream, overwrite: true);

        _logger.LogInformation("Thumbnail created for event {EventId}: {BlobName}", eventIdOrSessionId, thumbnailFileName);

        return _containerClient.Uri.AbsoluteUri.TrimEnd('/') + "/" + thumbnailFileName;
    }

    private static string? ExtractBlobNameFromUrl(string imageUrl)
    {
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
