using Events.Models.Enums;
using Events.Services.Media;
using Microsoft.Extensions.Logging.Abstractions;

namespace Events.Services.Tests.Media;

public class MediaImageStoreTests
{
    private readonly InMemoryBlobStorage _storage = new();
    private readonly MediaImageStore _store;

    public MediaImageStoreTests()
    {
        _store = new MediaImageStore(_storage, new ImageVariantGenerator(), NullLogger<MediaImageStore>.Instance);
    }

    [Fact]
    public async Task StoreAsync_NewImage_WritesOriginalAndBothVariantsWithContentTypes()
    {
        var content = TestImages.Png(1600, 900);
        var id = MediaImageStore.ComputeImageId(content);

        var result = await _store.StoreAsync(content, requireMinimumSize: true);

        Assert.True(result.IsSuccess);
        Assert.False(result.Reused);
        Assert.Equal($"/media/1200/{id}.webp", result.LargeUrl);
        Assert.Equal($"/media/480/{id}.webp", result.ThumbnailUrl);
        Assert.Equal(content, _storage.Blobs[$"original/{id}.png"].Content);
        Assert.Equal("image/png", _storage.Blobs[$"original/{id}.png"].ContentType);
        Assert.Equal("image/webp", _storage.Blobs[$"1200/{id}.webp"].ContentType);
        Assert.Equal("image/webp", _storage.Blobs[$"480/{id}.webp"].ContentType);
        Assert.Equal(3, _storage.Blobs.Count);
    }

    [Fact]
    public async Task StoreAsync_SameContentTwice_ReusesWithoutWriting()
    {
        var content = TestImages.Jpeg(800, 600);
        await _store.StoreAsync(content, requireMinimumSize: true);
        var uploadsAfterFirst = _storage.UploadCount;

        var second = await _store.StoreAsync(content, requireMinimumSize: true);

        Assert.True(second.IsSuccess);
        Assert.True(second.Reused);
        Assert.Equal(uploadsAfterFirst, _storage.UploadCount);
    }

    [Fact]
    public async Task StoreAsync_NotAnImage_ReturnsUnsupportedTypeAndWritesNothing()
    {
        var result = await _store.StoreAsync("<html>login</html>"u8.ToArray(), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.UnsupportedType, result.FailureReason);
        Assert.Empty(_storage.Blobs);
    }

    [Fact]
    public async Task StoreAsync_TooSmall_ReturnsTooSmallAndWritesNothing()
    {
        var result = await _store.StoreAsync(TestImages.Jpeg(200, 150), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.TooSmall, result.FailureReason);
        Assert.Empty(_storage.Blobs);
    }

    [Fact]
    public async Task StoreAsync_StorageFails_ReturnsProcessingError()
    {
        _storage.FailUploads = true;

        var result = await _store.StoreAsync(TestImages.Jpeg(800, 600), requireMinimumSize: true);

        Assert.Equal(ImageFetchFailureReason.ProcessingError, result.FailureReason);
    }

    [Fact]
    public async Task DeleteAsync_RemovesWholeSetAndAllowsStoringAgain()
    {
        var content = TestImages.Jpeg(800, 600);
        var stored = await _store.StoreAsync(content, requireMinimumSize: true);

        await _store.DeleteAsync(stored.ImageId!);

        Assert.Empty(_storage.Blobs);
        var again = await _store.StoreAsync(content, requireMinimumSize: true);
        Assert.False(again.Reused);
        Assert.Equal(3, _storage.Blobs.Count);
    }

    [Fact]
    public async Task DeleteAsync_KeepsOtherImages()
    {
        var first = await _store.StoreAsync(TestImages.Jpeg(800, 600), requireMinimumSize: true);
        await _store.StoreAsync(TestImages.Png(800, 600), requireMinimumSize: true);

        await _store.DeleteAsync(first.ImageId!);

        Assert.Equal(3, _storage.Blobs.Count);
        Assert.DoesNotContain(_storage.Blobs.Keys, k => k.Contains(first.ImageId!));
    }

    [Fact]
    public void ComputeImageId_Is32LowercaseHexAndStable()
    {
        var content = TestImages.Png(10, 10);

        var id = MediaImageStore.ComputeImageId(content);

        Assert.Matches("^[0-9a-f]{32}$", id);
        Assert.Equal(id, MediaImageStore.ComputeImageId(content.ToArray()));
        Assert.NotEqual(id, MediaImageStore.ComputeImageId(TestImages.Png(11, 10)));
    }

    private sealed class InMemoryBlobStorage : IMediaBlobStorage
    {
        public Dictionary<string, (byte[] Content, string ContentType)> Blobs { get; } = new();
        public int UploadCount { get; private set; }
        public bool FailUploads { get; set; }

        public Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default) =>
            Task.FromResult(Blobs.ContainsKey(blobName));

        public Task UploadAsync(string blobName, byte[] content, string contentType, CancellationToken cancellationToken = default)
        {
            if (FailUploads)
            {
                throw new InvalidOperationException("Storage unavailable");
            }

            UploadCount++;
            Blobs[blobName] = (content, contentType);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default) =>
            Task.FromResult(Blobs.Remove(blobName));

        public Task<int> DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            var names = Blobs.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
            names.ForEach(n => Blobs.Remove(n));
            return Task.FromResult(names.Count);
        }
    }
}
