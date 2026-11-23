using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Events.Data.Repositories.Interfaces;
using Events.Models.Enums;
using Events.Services.Implementations;
using Events.Services.Media;
using Events.Services.Tests.Media;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Events.Services.Tests.Implementations;

// Event images stored by content (GS-REQ-009, 9.1.3, 9.1.4, 9.1.6, 9.4): storing, reuse and deletion rules.
public class AzureBlobImageServiceTests
{
    private const string ImageId = "0123456789abcdef0123456789abcdef";
    private const string LargeUrl = "/media/1200/" + ImageId + ".webp";
    private const string ThumbUrl = "/media/480/" + ImageId + ".webp";

    private readonly InMemoryContainer _container = new();
    private readonly Mock<IEventRepository> _repository = new();
    private readonly AzureBlobImageService _service;

    public AzureBlobImageServiceTests()
    {
        _service = new AzureBlobImageService(
            _container.Client.Object, new ImageVariantGenerator(), _repository.Object, NullLogger<AzureBlobImageService>.Instance);
    }

    // StoreEventImageAsync

    [Fact]
    public async Task StoreEventImageAsync_NewImage_WritesOriginalAndBothVariantsWithContentTypes()
    {
        var content = TestImages.Png(1600, 900);
        var id = MediaImagePaths.ComputeImageId(content);

        var result = await _service.StoreEventImageAsync(content, requireMinimumSize: true);

        Assert.True(result.IsSuccess);
        Assert.False(result.Reused);
        Assert.Equal($"/media/1200/{id}.webp", result.LargeUrl);
        Assert.Equal($"/media/480/{id}.webp", result.ThumbnailUrl);
        Assert.Equal(content, _container.Blobs[$"original/{id}.png"].Content);
        Assert.Equal("image/png", _container.Blobs[$"original/{id}.png"].ContentType);
        Assert.Equal("image/webp", _container.Blobs[$"1200/{id}.webp"].ContentType);
        Assert.Equal("image/webp", _container.Blobs[$"480/{id}.webp"].ContentType);
        Assert.Equal(3, _container.Blobs.Count);
    }

    [Fact]
    public async Task StoreEventImageAsync_SameContentTwice_ReusesWithoutWriting()
    {
        var content = TestImages.Jpeg(800, 600);
        await _service.StoreEventImageAsync(content, requireMinimumSize: true);
        var uploadsAfterFirst = _container.UploadCount;

        var second = await _service.StoreEventImageAsync(content, requireMinimumSize: true);

        Assert.True(second.IsSuccess);
        Assert.True(second.Reused);
        Assert.Equal(uploadsAfterFirst, _container.UploadCount);
    }

    [Fact]
    public async Task StoreEventImageAsync_NotAnImage_ReturnsUnsupportedTypeAndWritesNothing()
    {
        var result = await _service.StoreEventImageAsync("<html>login</html>"u8.ToArray(), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.UnsupportedType, result.FailureReason);
        Assert.Empty(_container.Blobs);
    }

    [Fact]
    public async Task StoreEventImageAsync_TooSmall_ReturnsTooSmallAndWritesNothing()
    {
        var result = await _service.StoreEventImageAsync(TestImages.Jpeg(200, 150), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.TooSmall, result.FailureReason);
        Assert.Empty(_container.Blobs);
    }

    [Fact]
    public async Task StoreEventImageAsync_StorageFails_ReturnsProcessingError()
    {
        _container.FailUploads = true;

        var result = await _service.StoreEventImageAsync(TestImages.Jpeg(800, 600), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.ProcessingError, result.FailureReason);
    }

    // DeleteEventImagesAsync

    [Fact]
    public async Task DeleteEventImagesAsync_ImageUsedByAnotherEvent_KeepsIt()
    {
        _container.AddBlob($"480/{ImageId}.webp");
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(1);

        await _service.DeleteEventImagesAsync(5, LargeUrl, ThumbUrl);

        Assert.Single(_container.Blobs);
        Assert.Equal(0, _container.DeleteCount);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_ImageNotUsedElsewhere_DeletesWholeSetAndAllowsStoringAgain()
    {
        var content = TestImages.Jpeg(800, 600);
        var stored = await _service.StoreEventImageAsync(content, requireMinimumSize: true);
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(stored.LargeUrl!, stored.ThumbnailUrl!, 5)).ReturnsAsync(0);

        await _service.DeleteEventImagesAsync(5, stored.LargeUrl, stored.ThumbnailUrl);

        Assert.Empty(_container.Blobs);
        var again = await _service.StoreEventImageAsync(content, requireMinimumSize: true);
        Assert.False(again.Reused);
        Assert.Equal(3, _container.Blobs.Count);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_KeepsOtherImages()
    {
        var first = await _service.StoreEventImageAsync(TestImages.Jpeg(800, 600), requireMinimumSize: true);
        await _service.StoreEventImageAsync(TestImages.Png(800, 600), requireMinimumSize: true);
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(first.LargeUrl!, first.ThumbnailUrl!, 5)).ReturnsAsync(0);

        await _service.DeleteEventImagesAsync(5, first.LargeUrl, first.ThumbnailUrl);

        Assert.Equal(3, _container.Blobs.Count);
        Assert.DoesNotContain(_container.Blobs.Keys, k => k.Contains(first.ImageId!));
    }

    [Fact]
    public async Task DeleteEventImagesAsync_OnlyThumbnailIsNewScheme_StillChecksAndDeletesBySet()
    {
        _container.AddBlob($"480/{ImageId}.webp");
        _container.AddBlob($"1200/{ImageId}.webp");
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(0);

        await _service.DeleteEventImagesAsync(5, null, ThumbUrl);

        Assert.Empty(_container.Blobs);
    }

    [Theory]
    [InlineData("/media/original/12_638000000000000000.jpg", "/media/thumbnails/12_638000000000000000.jpg")]
    [InlineData("https://www.eventim.bg/obj/media/photo.jpg", null)]
    [InlineData(null, null)]
    public async Task DeleteEventImagesAsync_OlderOrExternalImage_DoesNotCheckSharedUse(string? imageUrl, string? thumbnailUrl)
    {
        await _service.DeleteEventImagesAsync(5, imageUrl, thumbnailUrl);

        _repository.Verify(r => r.CountOtherEventsUsingImageAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_StorageFails_DoesNotThrow()
    {
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(0);
        _container.FailDeletes = true;

        await _service.DeleteEventImagesAsync(5, LargeUrl, ThumbUrl);
    }

    // BlobContainerClient mocked over a dictionary: the Azure SDK clients are mockable, so the service
    // needs no storage abstraction of its own to be tested.
    private sealed class InMemoryContainer
    {
        public Mock<BlobContainerClient> Client { get; } = new();
        public Dictionary<string, (byte[] Content, string ContentType)> Blobs { get; } = new();
        public int UploadCount { get; private set; }
        public int DeleteCount { get; private set; }
        public bool FailUploads { get; set; }
        public bool FailDeletes { get; set; }

        public InMemoryContainer()
        {
            Client.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns((string name) => CreateBlobClient(name));
            Client.Setup(c => c.DeleteBlobIfExistsAsync(
                    It.IsAny<string>(), It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .Returns((string name, DeleteSnapshotsOption _, BlobRequestConditions _, CancellationToken _) =>
                {
                    if (FailDeletes)
                    {
                        throw new RequestFailedException("Storage unavailable");
                    }

                    DeleteCount++;
                    return Task.FromResult(Response.FromValue(Blobs.Remove(name), Mock.Of<Response>()));
                });
        }

        public void AddBlob(string name) => Blobs[name] = ([1], "image/webp");

        private BlobClient CreateBlobClient(string name)
        {
            var blob = new Mock<BlobClient>();
            blob.Setup(b => b.ExistsAsync(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(Response.FromValue(Blobs.ContainsKey(name), Mock.Of<Response>())));
            blob.Setup(b => b.UploadAsync(It.IsAny<BinaryData>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
                .Returns((BinaryData data, BlobUploadOptions options, CancellationToken _) =>
                {
                    if (FailUploads)
                    {
                        throw new RequestFailedException("Storage unavailable");
                    }

                    UploadCount++;
                    Blobs[name] = (data.ToArray(), options.HttpHeaders.ContentType);
                    return Task.FromResult(Mock.Of<Response<BlobContentInfo>>());
                });
            return blob.Object;
        }
    }
}
