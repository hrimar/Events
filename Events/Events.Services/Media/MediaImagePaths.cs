using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Events.Services.Media;

// Blob names and public addresses of images stored by content (GS-REQ-009, 9.1.4, 9.4):
// original/{id}.{ext} (archive, not shown), 1200/{id}.webp ("large") and 480/{id}.webp ("thumb"),
// served by MediaController under /media/... (GS-REQ-003).
public static partial class MediaImagePaths
{
    public const string UrlPrefix = "/media/";

    private const int ImageIdLength = 32;

    // Image id = first 32 hex characters of the SHA-256 of the original bytes (GS-REQ-009, 9.1.6).
    public static string ComputeImageId(byte[] content) => Convert.ToHexString(SHA256.HashData(content))[..ImageIdLength].ToLowerInvariant();

    public static string OriginalBlobName(string imageId, DetectedImageFormat format) =>
        $"original/{imageId}.{ImageFormatSniffer.GetExtension(format)}";

    public static string LargeBlobName(string imageId) => $"{ImageVariantGenerator.LargeWidth}/{imageId}.webp";

    public static string ThumbnailBlobName(string imageId) => $"{ImageVariantGenerator.ThumbnailWidth}/{imageId}.webp";

    public static string ToUrl(string blobName) => UrlPrefix + blobName;

    // Image id from a "/media/480/{id}.webp" or "/media/1200/{id}.webp" address; null for any other address
    // (older uploads, external images from the ticket platforms).
    public static string? TryGetImageId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var match = VariantUrlRegex().Match(url);
        return match.Success ? match.Groups["id"].Value : null;
    }

    [GeneratedRegex("^/media/(?:480|1200)/(?<id>[0-9a-f]{32})\\.webp$")]
    private static partial Regex VariantUrlRegex();
}
