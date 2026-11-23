using Events.Models.Enums;

namespace Events.Services.Media;

// Result of storing one image by content (GS-REQ-009, 9.1.4, 9.1.6).
public sealed class MediaStoreResult
{
    public bool IsSuccess => FailureReason == null;

    // True when the same content was already stored and nothing new was written (GS-REQ-009, 9.1.6).
    public bool Reused { get; private init; }

    public string? ImageId { get; private init; }

    // "/media/1200/{id}.webp", used as Event.ImageUrl.
    public string? LargeUrl { get; private init; }

    // "/media/480/{id}.webp", used as Event.ThumbnailUrl.
    public string? ThumbnailUrl { get; private init; }

    public ImageFetchFailureReason? FailureReason { get; private init; }

    public string? Detail { get; private init; }

    public static MediaStoreResult Success(string imageId, bool reused) => new()
    {
        ImageId = imageId,
        Reused = reused,
        LargeUrl = MediaImagePaths.ToUrl(MediaImagePaths.LargeBlobName(imageId)),
        ThumbnailUrl = MediaImagePaths.ToUrl(MediaImagePaths.ThumbnailBlobName(imageId))
    };

    public static MediaStoreResult Failure(ImageFetchFailureReason reason, string detail) =>
        new() { FailureReason = reason, Detail = detail };
}
