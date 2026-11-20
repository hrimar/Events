using Events.Models.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Events.Services.Media;

// Builds the "thumb" and "large" WebP variants of a stored image (GS-REQ-009, 9.4):
// width only (height keeps the aspect ratio), never upscaled, GIF reduced to its first frame (9.1.3),
// EXIF orientation applied and EXIF/XMP metadata dropped.
public sealed class ImageVariantGenerator
{
    public const int ThumbnailWidth = 480;
    public const int ThumbnailQuality = 80;
    public const int LargeWidth = 1200;
    public const int LargeQuality = 82;

    // Minimum source size for imported images (GS-REQ-009, 9.1.3).
    public const int MinWidth = 300;
    public const int MinHeight = 200;

    // Upper bound on decoded size: a small file can declare huge dimensions (decompression bomb).
    public const long MaxPixels = 50_000_000;

    // Only the first frame is decoded, so animated GIFs become still images (GS-REQ-009, 9.1.3).
    private static readonly DecoderOptions FirstFrameOnly = new() { MaxFrames = 1 };

    // requireMinimumSize: true for the Excel import; false for the admin upload, which keeps its current rules.
    public ImageVariantResult Generate(byte[] content, bool requireMinimumSize)
    {
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            var info = Image.Identify(FirstFrameOnly, content);
            if ((long)info.Width * info.Height > MaxPixels)
            {
                return ImageVariantResult.Failure(ImageFetchFailureReason.TooLarge, $"{info.Width}x{info.Height}px is over {MaxPixels / 1_000_000} megapixels.");
            }

            using var image = Image.Load(FirstFrameOnly, content);
            image.Mutate(x => x.AutoOrient());
            image.Metadata.ExifProfile = null;
            image.Metadata.XmpProfile = null;

            if (requireMinimumSize && (image.Width < MinWidth || image.Height < MinHeight))
            {
                return ImageVariantResult.Failure(ImageFetchFailureReason.TooSmall, $"{image.Width}x{image.Height}px, minimum is {MinWidth}x{MinHeight}px.");
            }

            var thumbnail = EncodeVariant(image, ThumbnailWidth, ThumbnailQuality);
            var large = EncodeVariant(image, LargeWidth, LargeQuality);

            return ImageVariantResult.Success(thumbnail, large, image.Width, image.Height);
        }
        catch (Exception ex) when (ex is ImageFormatException or NotSupportedException)
        {
            return ImageVariantResult.Failure(ImageFetchFailureReason.ProcessingError, $"Cannot decode the image: {ex.Message}");
        }
    }

    private static byte[] EncodeVariant(Image source, int targetWidth, int quality)
    {
        // Smaller images are not upscaled (GS-REQ-009, 9.4).
        using var variant = source.Width > targetWidth ? source.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new Size(targetWidth, 0),
            Mode = ResizeMode.Max,
            Sampler = KnownResamplers.Lanczos3
        }))
            : source.Clone(_ => { });

        using var output = new MemoryStream();
        variant.SaveAsWebp(output, new WebpEncoder { FileFormat = WebpFileFormatType.Lossy, Quality = quality });
        return output.ToArray();
    }
}
