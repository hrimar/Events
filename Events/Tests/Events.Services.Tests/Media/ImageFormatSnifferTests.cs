using System.Text;
using Events.Services.Media;

namespace Events.Services.Tests.Media;

public class ImageFormatSnifferTests
{
    [Fact]
    public void Detect_RealImages_ReturnsFormat()
    {
        Assert.Equal(DetectedImageFormat.Jpeg, ImageFormatSniffer.Detect(TestImages.Jpeg(10, 10)));
        Assert.Equal(DetectedImageFormat.Png, ImageFormatSniffer.Detect(TestImages.Png(10, 10)));
        Assert.Equal(DetectedImageFormat.WebP, ImageFormatSniffer.Detect(TestImages.Webp(10, 10)));
        Assert.Equal(DetectedImageFormat.Gif, ImageFormatSniffer.Detect(TestImages.Gif(10, 10)));
    }

    [Fact]
    public void Detect_Gif87a_ReturnsGif()
    {
        Assert.Equal(DetectedImageFormat.Gif, ImageFormatSniffer.Detect(Encoding.ASCII.GetBytes("GIF87a....")));
    }

    [Theory]
    [InlineData("<!DOCTYPE html><html></html>")]
    [InlineData("<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>")]
    [InlineData("RIFF\0\0\0\0WAVEfmt ")]
    [InlineData("RIFF\0\0")]
    [InlineData("%PDF-1.7")]
    [InlineData("")]
    public void Detect_NotAcceptedContent_ReturnsNull(string content)
    {
        Assert.Null(ImageFormatSniffer.Detect(Encoding.ASCII.GetBytes(content)));
    }

    [Fact]
    public void Detect_IgnoresWhatTheNameOrHeaderSays_OnlyBytesCount()
    {
        // A PNG body is a PNG even if served as "photo.jpg" with Content-Type image/jpeg.
        Assert.Equal(DetectedImageFormat.Png, ImageFormatSniffer.Detect(TestImages.Png(5, 5)));
        Assert.Null(ImageFormatSniffer.Detect(new byte[] { 0xFF, 0xD8 }));
    }

    [Theory]
    [InlineData(DetectedImageFormat.Jpeg, "jpg", "image/jpeg")]
    [InlineData(DetectedImageFormat.Png, "png", "image/png")]
    [InlineData(DetectedImageFormat.WebP, "webp", "image/webp")]
    [InlineData(DetectedImageFormat.Gif, "gif", "image/gif")]
    public void ExtensionAndContentType_MatchFormat(DetectedImageFormat format, string extension, string contentType)
    {
        Assert.Equal(extension, ImageFormatSniffer.GetExtension(format));
        Assert.Equal(contentType, ImageFormatSniffer.GetContentType(format));
    }
}
