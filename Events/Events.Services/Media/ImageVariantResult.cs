using Events.Models.Enums;

namespace Events.Services.Media;

// The two WebP variants of one image, or why they could not be made (GS-REQ-009, 9.1.3, 9.4).
public sealed class ImageVariantResult
{
    public bool IsSuccess => FailureReason == null;

    // "thumb": 480px wide WebP (cards, tiles).
    public byte[] Thumbnail { get; private init; } = Array.Empty<byte>();

    // "large": 1200px wide WebP (hero, event page, og:image).
    public byte[] Large { get; private init; } = Array.Empty<byte>();

    // Source size after EXIF orientation.
    public int Width { get; private init; }

    public int Height { get; private init; }

    public ImageFetchFailureReason? FailureReason { get; private init; }

    public string? Detail { get; private init; }

    public static ImageVariantResult Success(byte[] thumbnail, byte[] large, int width, int height) =>
        new() { Thumbnail = thumbnail, Large = large, Width = width, Height = height };

    public static ImageVariantResult Failure(ImageFetchFailureReason reason, string detail) =>
        new() { FailureReason = reason, Detail = detail };
}
