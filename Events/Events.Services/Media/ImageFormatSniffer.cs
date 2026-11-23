namespace Events.Services.Media;

// Image formats accepted for local copies (GS-REQ-009, 9.1.3).
public enum DetectedImageFormat
{
    Jpeg = 1,
    Png = 2,
    WebP = 3,
    Gif = 4
}

// Detects the image type from the real content (magic bytes), never from the extension or Content-Type
// (GS-REQ-009, 9.1.3).
public static class ImageFormatSniffer
{
    private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] Gif87Signature = "GIF87a"u8.ToArray();
    private static readonly byte[] Gif89Signature = "GIF89a"u8.ToArray();
    private static readonly byte[] RiffSignature = "RIFF"u8.ToArray();
    private static readonly byte[] WebpSignature = "WEBP"u8.ToArray();

    // Null when the content is none of the accepted formats.
    public static DetectedImageFormat? Detect(ReadOnlySpan<byte> content)
    {
        if (content.StartsWith(JpegSignature))
        {
            return DetectedImageFormat.Jpeg;
        }

        if (content.StartsWith(PngSignature))
        {
            return DetectedImageFormat.Png;
        }

        if (content.StartsWith(Gif87Signature) || content.StartsWith(Gif89Signature))
        {
            return DetectedImageFormat.Gif;
        }

        // "RIFF" + 4-byte size + "WEBP" (other RIFF files, e.g. WAV, have a different form type).
        if (content.Length >= 12 && content.StartsWith(RiffSignature) && content.Slice(8, 4).SequenceEqual(WebpSignature))
        {
            return DetectedImageFormat.WebP;
        }

        return null;
    }

    public static string GetExtension(DetectedImageFormat format) => format switch
    {
        DetectedImageFormat.Jpeg => "jpg",
        DetectedImageFormat.Png => "png",
        DetectedImageFormat.WebP => "webp",
        DetectedImageFormat.Gif => "gif",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };

    public static string GetContentType(DetectedImageFormat format) => format switch
    {
        DetectedImageFormat.Jpeg => "image/jpeg",
        DetectedImageFormat.Png => "image/png",
        DetectedImageFormat.WebP => "image/webp",
        DetectedImageFormat.Gif => "image/gif",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
}
