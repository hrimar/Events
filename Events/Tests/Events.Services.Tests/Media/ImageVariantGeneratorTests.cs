using Events.Models.Enums;
using Events.Services.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace Events.Services.Tests.Media;

public class ImageVariantGeneratorTests
{
    private readonly ImageVariantGenerator _generator = new();

    [Fact]
    public void Generate_WideImage_ResizesByWidthKeepingAspectRatio()
    {
        var result = _generator.Generate(TestImages.Jpeg(2000, 1000), requireMinimumSize: true);

        Assert.True(result.IsSuccess);
        AssertWebp(result.Thumbnail, 480, 240);
        AssertWebp(result.Large, 1200, 600);
    }

    [Fact]
    public void Generate_ImageNarrowerThanLarge_IsNotUpscaled()
    {
        var result = _generator.Generate(TestImages.Png(800, 600), requireMinimumSize: true);

        AssertWebp(result.Thumbnail, 480, 360);
        AssertWebp(result.Large, 800, 600);
    }

    [Fact]
    public void Generate_ImageNarrowerThanThumbnail_KeepsSizeInBothVariants()
    {
        var result = _generator.Generate(TestImages.Webp(400, 300), requireMinimumSize: true);

        AssertWebp(result.Thumbnail, 400, 300);
        AssertWebp(result.Large, 400, 300);
    }

    [Theory]
    [InlineData(299, 400)]
    [InlineData(600, 199)]
    public void Generate_BelowMinimumSize_ReturnsTooSmallWhenRequired(int width, int height)
    {
        var result = _generator.Generate(TestImages.Jpeg(width, height), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.TooSmall, result.FailureReason);
    }

    [Fact]
    public void Generate_BelowMinimumSize_AllowedForAdminUpload()
    {
        var result = _generator.Generate(TestImages.Jpeg(120, 80), requireMinimumSize: false);

        Assert.True(result.IsSuccess);
        AssertWebp(result.Large, 120, 80);
    }

    [Fact]
    public void Generate_ExactlyMinimumSize_Succeeds()
    {
        Assert.True(_generator.Generate(TestImages.Png(300, 200), requireMinimumSize: true).IsSuccess);
    }

    [Fact]
    public void Generate_AnimatedGif_UsesFirstFrameOnly()
    {
        var result = _generator.Generate(TestImages.AnimatedGif(600, 400, frames: 3), requireMinimumSize: true);

        Assert.True(result.IsSuccess);
        using var large = Image.Load(result.Large);
        Assert.Equal(1, large.Frames.Count);
    }

    [Fact]
    public void Generate_ExifRotated_AppliesOrientationAndDropsExif()
    {
        byte[] rotated;
        using (var image = new Image<Rgba32>(800, 400, Color.Green))
        {
            image.Metadata.ExifProfile = new ExifProfile();
            image.Metadata.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6); // rotate 90 degrees clockwise
            using var output = new MemoryStream();
            image.SaveAsJpeg(output);
            rotated = output.ToArray();
        }

        var result = _generator.Generate(rotated, requireMinimumSize: true);

        AssertWebp(result.Large, 400, 800);
        using var large = Image.Load(result.Large);
        Assert.Null(large.Metadata.ExifProfile);
    }

    [Fact]
    public void Generate_HugeDeclaredDimensions_ReturnsTooLargeWithoutDecoding()
    {
        var result = _generator.Generate(TestImages.PngWithDeclaredSize(10_000, 10_000), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.TooLarge, result.FailureReason);
    }

    [Fact]
    public void Generate_CorruptImage_ReturnsProcessingError()
    {
        var corrupt = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x02, 0x03 };

        var result = _generator.Generate(corrupt, requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.ProcessingError, result.FailureReason);
    }

    private static void AssertWebp(byte[] content, int expectedWidth, int expectedHeight)
    {
        Assert.Equal(DetectedImageFormat.WebP, ImageFormatSniffer.Detect(content));
        var info = Image.Identify(content);
        Assert.Equal(expectedWidth, info.Width);
        Assert.Equal(expectedHeight, info.Height);
    }
}
