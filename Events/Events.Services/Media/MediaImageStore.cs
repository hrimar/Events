using System.Security.Cryptography;
using Events.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Events.Services.Media;

// Stores images by content (GS-REQ-009, 9.1.3, 9.1.4, 9.1.6, 9.4). The image id is the first 32 hex characters
// of the SHA-256 of the original bytes, so the same image always gets the same names and a second import
// of the same content writes nothing (acceptance criterion 4 of GS-REQ-009).
public sealed class MediaImageStore : IMediaImageStore
{
    private const int ImageIdLength = 32;

    private readonly IMediaBlobStorage _storage;
    private readonly ImageVariantGenerator _variantGenerator;
    private readonly ILogger<MediaImageStore> _logger;

    public MediaImageStore(IMediaBlobStorage storage, ImageVariantGenerator variantGenerator, ILogger<MediaImageStore> logger)
    {
        _storage = storage;
        _variantGenerator = variantGenerator;
        _logger = logger;
    }

    public async Task<MediaStoreResult> StoreAsync(byte[] content, bool requireMinimumSize, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        var format = ImageFormatSniffer.Detect(content);
        if (format == null)
        {
            return MediaStoreResult.Failure(ImageFetchFailureReason.UnsupportedType, "Content is not JPEG, PNG, WebP or GIF.");
        }

        var imageId = ComputeImageId(content);

        // The thumbnail is written last, so its presence means the whole set is complete.
        if (await _storage.ExistsAsync(MediaImagePaths.ThumbnailBlobName(imageId), cancellationToken))
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
            await _storage.UploadAsync(
                MediaImagePaths.OriginalBlobName(imageId, format.Value), content, ImageFormatSniffer.GetContentType(format.Value), cancellationToken);
            await _storage.UploadAsync(MediaImagePaths.LargeBlobName(imageId), variants.Large, "image/webp", cancellationToken);
            await _storage.UploadAsync(MediaImagePaths.ThumbnailBlobName(imageId), variants.Thumbnail, "image/webp", cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to write image {ImageId} to storage", imageId);
            return MediaStoreResult.Failure(ImageFetchFailureReason.ProcessingError, "Could not write the image to storage.");
        }

        _logger.LogInformation("Stored image {ImageId} ({Format}, {Width}x{Height}px)", imageId, format, variants.Width, variants.Height);
        return MediaStoreResult.Success(imageId, reused: false);
    }

    public static string ComputeImageId(byte[] content) => Convert.ToHexString(SHA256.HashData(content))[..ImageIdLength].ToLowerInvariant();
}
