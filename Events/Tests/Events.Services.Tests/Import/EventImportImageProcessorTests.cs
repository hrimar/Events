using System.Net;
using System.Text;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Import;
using Events.Services.Interfaces;
using Events.Services.Media;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Events.Services.Tests.Import;

public class EventImportImageProcessorTests
{
    private const string ImageUrl = "https://scontent.fbcdn.net/v/photo.jpg?oh=abc&oe=6A1B2C3D";
    private const string PageUrl = "https://tickets.example.bg/events/jazz";
    private const string ImageId = "0123456789abcdef0123456789abcdef";
    private static readonly byte[] ImageBytes = { 0xFF, 0xD8, 0xFF, 0xE0 };

    // Before the "oe" expiry of ImageUrl (2026-05-30).
    private readonly SettableTimeProvider _time = new(new DateTimeOffset(2026, 5, 1, 12, 0, 0, TimeSpan.Zero));
    private readonly Mock<ISafeHttpFetcher> _fetcher = new();
    private readonly Mock<IImageUploadService> _store = new();
    private readonly Mock<IEventService> _eventService = new();

    public EventImportImageProcessorTests()
    {
        _eventService.Setup(s => s.SetEventImageAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private EventImportImageProcessor CreateProcessor() => new(
        _fetcher.Object, _store.Object, _eventService.Object, _time, NullLogger<EventImportImageProcessor>.Instance);

    private static EventImportJobItem PendingItem(string? imageUrl = ImageUrl, string? pageUrl = null) => new()
    {
        JobId = 1,
        RowNumber = 2,
        EventId = 10,
        EventName = "Jazz",
        SourceImageUrl = imageUrl,
        PageUrl = pageUrl
    };

    [Fact]
    public async Task ProcessAsync_ImageUrl_StoresAndLinksLocalCopyKeepingSourceAddress()
    {
        SetupImageDownload(ImageUrl);
        SetupStore(reused: false);
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Downloaded, item.Outcome);
        Assert.False(item.ImageFromPage);
        Assert.Null(item.FailureReason);
        Assert.NotNull(item.ProcessedAtUtc);
        _store.Verify(s => s.StoreEventImageAsync(ImageBytes, true, It.IsAny<CancellationToken>()), Times.Once);
        _eventService.Verify(s => s.SetEventImageAsync(
            10, $"/media/1200/{ImageId}.webp", $"/media/480/{ImageId}.webp", ImageUrl, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_SameImageAlreadyStored_IsReused()
    {
        SetupImageDownload(ImageUrl);
        SetupStore(reused: true);
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Reused, item.Outcome);
    }

    [Fact]
    public async Task ProcessAsync_OnlyPageUrl_UsesImageFoundOnPage()
    {
        const string pageImage = "https://cdn.example.bg/jazz.jpg";
        SetupPage($"""<meta property="og:image" content="{pageImage}">""");
        SetupImageDownload(pageImage);
        SetupStore(reused: false);
        var item = PendingItem(imageUrl: null, pageUrl: PageUrl);

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Downloaded, item.Outcome);
        Assert.True(item.ImageFromPage);
        _eventService.Verify(s => s.SetEventImageAsync(
            10, It.IsAny<string>(), It.IsAny<string>(), pageImage, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_PageWithoutImage_FailsWithNoImageOnPage()
    {
        SetupPage("<html><head><title>Facebook</title></head></html>");
        var item = PendingItem(imageUrl: null, pageUrl: "https://www.facebook.com/events/123/");

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Failed, item.Outcome);
        Assert.Equal(ImageFetchFailureReason.NoImageOnPage, item.FailureReason);
        _fetcher.Verify(f => f.FetchImageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_PageCannotBeFetched_FailsWithPageReason()
    {
        _fetcher.Setup(f => f.FetchPageAsync(PageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Failure(ImageFetchFailureReason.Timeout, "No complete response."));
        var item = PendingItem(imageUrl: null, pageUrl: PageUrl);

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(ImageFetchFailureReason.Timeout, item.FailureReason);
        Assert.StartsWith("Ticket URL page:", item.Detail);
    }

    [Theory]
    [InlineData(ImageFetchFailureReason.ForbiddenAddress)]
    [InlineData(ImageFetchFailureReason.NotHttps)]
    [InlineData(ImageFetchFailureReason.TooLarge)]
    [InlineData(ImageFetchFailureReason.Timeout)]
    public async Task ProcessAsync_DownloadRefused_KeepsReasonAndDoesNotTouchEvent(ImageFetchFailureReason reason)
    {
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Failure(reason, "refused"));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Failed, item.Outcome);
        Assert.Equal(reason, item.FailureReason);
        AssertEventNotUpdated();
    }

    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Gone)]
    public async Task ProcessAsync_FacebookAddressPastItsOe_IsReportedAsExpired(HttpStatusCode status)
    {
        _time.SetUtcNow(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Failure(ImageFetchFailureReason.HttpError, "HTTP 403.", new Uri(ImageUrl), status));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(ImageFetchFailureReason.Expired, item.FailureReason);
    }

    [Fact]
    public async Task ProcessAsync_FacebookAddressStillValidButRefused_KeepsHttpReason()
    {
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Failure(
                ImageFetchFailureReason.NotFound, "HTTP 404.", new Uri(ImageUrl), HttpStatusCode.NotFound));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(ImageFetchFailureReason.NotFound, item.FailureReason);
    }

    [Theory]
    [InlineData(ImageFetchFailureReason.UnsupportedType)]
    [InlineData(ImageFetchFailureReason.TooSmall)]
    public async Task ProcessAsync_StoreRejectsImage_FailsWithStoreReason(ImageFetchFailureReason reason)
    {
        SetupImageDownload(ImageUrl);
        _store.Setup(s => s.StoreEventImageAsync(ImageBytes, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediaStoreResult.Failure(reason, "rejected"));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(reason, item.FailureReason);
        AssertEventNotUpdated();
    }

    [Fact]
    public async Task ProcessAsync_EventDeletedMeanwhile_Fails()
    {
        SetupImageDownload(ImageUrl);
        SetupStore(reused: false);
        _eventService.Setup(s => s.SetEventImageAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(EventImportImageOutcome.Failed, item.Outcome);
        Assert.Equal(ImageFetchFailureReason.ProcessingError, item.FailureReason);
    }

    [Fact]
    public async Task ProcessAsync_UnexpectedException_IsRecordedNotThrown()
    {
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(ImageFetchFailureReason.ProcessingError, item.FailureReason);
        Assert.Equal("boom", item.Detail);
        Assert.NotNull(item.ProcessedAtUtc);
    }

    [Fact]
    public async Task ProcessAsync_CallerCancels_Throws()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>())).ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(() => CreateProcessor().ProcessAsync(PendingItem(), cts.Token));
    }

    [Theory]
    [InlineData(EventImportImageOutcome.Downloaded)]
    [InlineData(EventImportImageOutcome.NoImage)]
    [InlineData(EventImportImageOutcome.RowNotImported)]
    public async Task ProcessAsync_ItemNotPending_IsLeftUnchanged(EventImportImageOutcome outcome)
    {
        var item = PendingItem();
        item.Outcome = outcome;

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(outcome, item.Outcome);
        Assert.Null(item.ProcessedAtUtc);
        _fetcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ProcessAsync_LongErrorDetail_IsTruncatedTo500()
    {
        _fetcher.Setup(f => f.FetchImageAsync(ImageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Failure(ImageFetchFailureReason.HttpError, new string('x', 800)));
        var item = PendingItem();

        await CreateProcessor().ProcessAsync(item);

        Assert.Equal(500, item.Detail!.Length);
    }

    private void SetupImageDownload(string url) =>
        _fetcher.Setup(f => f.FetchImageAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RemoteFetchResult.Success(ImageBytes, "image/jpeg", new Uri(url), HttpStatusCode.OK));

    private void SetupPage(string html) =>
        _fetcher.Setup(f => f.FetchPageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string url, CancellationToken _) =>
                RemoteFetchResult.Success(Encoding.UTF8.GetBytes(html), "text/html", new Uri(url), HttpStatusCode.OK));

    private void SetupStore(bool reused) =>
        _store.Setup(s => s.StoreEventImageAsync(ImageBytes, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediaStoreResult.Success(ImageId, reused));

    private void AssertEventNotUpdated() =>
        _eventService.Verify(s => s.SetEventImageAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);

    // Minimal TimeProvider whose "now" the test can move (same approach as SofiaClockTests).
    private sealed class SettableTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public SettableTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public void SetUtcNow(DateTimeOffset utcNow) => _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
