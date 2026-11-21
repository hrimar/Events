using Azure.Storage.Blobs;
using Events.Data.Repositories.Interfaces;
using Events.Services.Implementations;
using Events.Services.Media;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Events.Services.Tests.Implementations;

// Deletion rules for event images stored by content (GS-REQ-009, 9.1.6, 9.4).
public class AzureBlobImageServiceTests
{
    private const string ImageId = "0123456789abcdef0123456789abcdef";
    private const string LargeUrl = "/media/1200/" + ImageId + ".webp";
    private const string ThumbUrl = "/media/480/" + ImageId + ".webp";

    private readonly Mock<IMediaImageStore> _store = new();
    private readonly Mock<IEventRepository> _repository = new();
    private readonly AzureBlobImageService _service;

    public AzureBlobImageServiceTests()
    {
        _service = new AzureBlobImageService(
            new Mock<BlobContainerClient>().Object, _store.Object, _repository.Object, NullLogger<AzureBlobImageService>.Instance);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_ImageUsedByAnotherEvent_KeepsIt()
    {
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(1);

        await _service.DeleteEventImagesAsync(5, LargeUrl, ThumbUrl);

        _store.Verify(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_ImageNotUsedElsewhere_DeletesWholeSet()
    {
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(0);

        await _service.DeleteEventImagesAsync(5, LargeUrl, ThumbUrl);

        _store.Verify(s => s.DeleteAsync(ImageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_OnlyThumbnailIsNewScheme_StillChecksAndDeletesBySet()
    {
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(0);

        await _service.DeleteEventImagesAsync(5, null, ThumbUrl);

        _store.Verify(s => s.DeleteAsync(ImageId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("/media/original/12_638000000000000000.jpg", "/media/thumbnails/12_638000000000000000.jpg")]
    [InlineData("https://www.eventim.bg/obj/media/photo.jpg", null)]
    [InlineData(null, null)]
    public async Task DeleteEventImagesAsync_OlderOrExternalImage_DoesNotTouchTheContentStore(string? imageUrl, string? thumbnailUrl)
    {
        await _service.DeleteEventImagesAsync(5, imageUrl, thumbnailUrl);

        _repository.Verify(r => r.CountOtherEventsUsingImageAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _store.Verify(s => s.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEventImagesAsync_StoreFails_DoesNotThrow()
    {
        _repository.Setup(r => r.CountOtherEventsUsingImageAsync(LargeUrl, ThumbUrl, 5)).ReturnsAsync(0);
        _store.Setup(s => s.DeleteAsync(ImageId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());

        await _service.DeleteEventImagesAsync(5, LargeUrl, ThumbUrl);
    }
}
