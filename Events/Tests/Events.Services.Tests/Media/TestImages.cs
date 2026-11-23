using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace Events.Services.Tests.Media;

// In-memory test images for the media tests (GS-REQ-009).
internal static class TestImages
{
    public static byte[] Jpeg(int width, int height) => Encode(width, height, new JpegEncoder());

    public static byte[] Png(int width, int height) => Encode(width, height, new PngEncoder());

    public static byte[] Webp(int width, int height) => Encode(width, height, new WebpEncoder());

    public static byte[] Gif(int width, int height) => Encode(width, height, new GifEncoder());

    public static byte[] AnimatedGif(int width, int height, int frames)
    {
        using var image = new Image<Rgba32>(width, height, Color.Red);
        for (var i = 1; i < frames; i++)
        {
            using var frame = new Image<Rgba32>(width, height, i % 2 == 0 ? Color.Red : Color.Blue);
            image.Frames.AddFrame(frame.Frames.RootFrame);
        }

        using var output = new MemoryStream();
        image.SaveAsGif(output);
        return output.ToArray();
    }

    // PNG whose header declares width x height while the pixel data is tiny (decompression bomb shape).
    public static byte[] PngWithDeclaredSize(int width, int height)
    {
        var bytes = Png(1, 1);

        // IHDR starts at 8: length(4) "IHDR"(4) width(4) height(4) ...; CRC covers type + 13 data bytes.
        WriteBigEndian(bytes, 16, (uint)width);
        WriteBigEndian(bytes, 20, (uint)height);
        WriteBigEndian(bytes, 29, Crc32(bytes, 12, 17));
        return bytes;
    }

    private static byte[] Encode(int width, int height, IImageEncoder encoder)
    {
        using var image = new Image<Rgba32>(width, height, Color.CornflowerBlue);
        using var output = new MemoryStream();
        image.Save(output, encoder);
        return output.ToArray();
    }

    private static void WriteBigEndian(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static uint Crc32(byte[] buffer, int offset, int count)
    {
        var crc = 0xFFFFFFFFu;
        for (var i = offset; i < offset + count; i++)
        {
            crc ^= buffer[i];
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320u : crc >> 1;
            }
        }

        return ~crc;
    }
}
