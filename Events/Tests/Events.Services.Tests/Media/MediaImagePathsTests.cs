using Events.Services.Media;

namespace Events.Services.Tests.Media;

public class MediaImagePathsTests
{
    private const string Id = "0123456789abcdef0123456789abcdef";

    [Fact]
    public void BlobNames_FollowTheVariantScheme()
    {
        Assert.Equal($"original/{Id}.gif", MediaImagePaths.OriginalBlobName(Id, DetectedImageFormat.Gif));
        Assert.Equal($"1200/{Id}.webp", MediaImagePaths.LargeBlobName(Id));
        Assert.Equal($"480/{Id}.webp", MediaImagePaths.ThumbnailBlobName(Id));
        Assert.Equal($"/media/480/{Id}.webp", MediaImagePaths.ToUrl(MediaImagePaths.ThumbnailBlobName(Id)));
    }

    [Theory]
    [InlineData("/media/480/" + Id + ".webp")]
    [InlineData("/media/1200/" + Id + ".webp")]
    public void TryGetImageId_VariantUrl_ReturnsId(string url)
    {
        Assert.Equal(Id, MediaImagePaths.TryGetImageId(url));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("/media/original/123_638000000000000000.jpg")]
    [InlineData("/media/thumbnails/123_638000000000000000.jpg")]
    [InlineData("/media/original/" + Id + ".jpg")]
    [InlineData("/media/800/" + Id + ".webp")]
    [InlineData("https://www.eventim.bg/obj/media/BG-eventim/teaser/222x222/2026/photo.jpg")]
    [InlineData("/media/480/" + Id + ".webp?x=1")]
    public void TryGetImageId_OtherUrl_ReturnsNull(string? url)
    {
        Assert.Null(MediaImagePaths.TryGetImageId(url));
    }

    [Fact]
    public void ComputeImageId_Is32LowercaseHexAndStable()
    {
        var content = TestImages.Png(10, 10);

        var id = MediaImagePaths.ComputeImageId(content);

        Assert.Matches("^[0-9a-f]{32}$", id);
        Assert.Equal(id, MediaImagePaths.ComputeImageId(content.ToArray()));
        Assert.NotEqual(id, MediaImagePaths.ComputeImageId(TestImages.Png(11, 10)));
    }
}
