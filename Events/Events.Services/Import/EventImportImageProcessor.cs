using System.Net;
using System.Text;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Media;
using Microsoft.Extensions.Logging;

namespace Events.Services.Import;

// One row of the Excel import image job (GS-REQ-009, 9.1, 9.2, 9.3.1):
// 1. "Image URL" if present; otherwise the "Ticket URL" page and its og:image / twitter:image / JSON-LD image (9.2);
// 2. safe download (9.1.1, 9.1.2), type and size checks, storage by content with reuse (9.1.3, 9.1.4, 9.1.6);
// 3. the event gets the local copy, the external address is kept only as ImageSourceUrl (9.1.5).
// Any failure leaves the event without an image (default image) and is recorded on the item (9.3.1).
public sealed class EventImportImageProcessor : IEventImportImageProcessor
{
    private const int DetailMaxLength = 500;

    private readonly ISafeHttpFetcher _fetcher;
    private readonly IImageUploadService _imageService;
    private readonly IEventService _eventService;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<EventImportImageProcessor> _logger;

    public EventImportImageProcessor(
        ISafeHttpFetcher fetcher,
        IImageUploadService imageService,
        IEventService eventService,
        TimeProvider timeProvider,
        ILogger<EventImportImageProcessor> logger)
    {
        _fetcher = fetcher;
        _imageService = imageService;
        _eventService = eventService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task ProcessAsync(EventImportJobItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Outcome != EventImportImageOutcome.Pending)
        {
            return;
        }

        try
        {
            await ProcessPendingAsync(item, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Unexpected error for import row {RowNumber} of job {JobId}", item.RowNumber, item.JobId);
            Fail(item, ImageFetchFailureReason.ProcessingError, ex.Message);
        }

        item.ProcessedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
    }

    private async Task ProcessPendingAsync(EventImportJobItem item, CancellationToken cancellationToken)
    {
        if (item.EventId == null)
        {
            Fail(item, ImageFetchFailureReason.ProcessingError, "The row has no imported event.");
            return;
        }

        var imageUrl = item.SourceImageUrl;
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            imageUrl = await FindImageOnPageAsync(item, cancellationToken);
            if (imageUrl == null)
            {
                return;
            }

            item.ImageFromPage = true;
        }

        var download = await _fetcher.FetchImageAsync(imageUrl, cancellationToken);
        if (!download.IsSuccess)
        {
            Fail(item, ClassifyDownloadFailure(imageUrl, download), download.Detail);
            return;
        }

        var stored = await _imageService.StoreEventImageAsync(download.Content, requireMinimumSize: true, cancellationToken);
        if (!stored.IsSuccess)
        {
            Fail(item, stored.FailureReason!.Value, stored.Detail);
            return;
        }

        var linked = await _eventService.SetEventImageAsync(item.EventId.Value, stored.LargeUrl!, stored.ThumbnailUrl!, imageUrl, cancellationToken);
        if (!linked)
        {
            Fail(item, ImageFetchFailureReason.ProcessingError, $"Event #{item.EventId} no longer exists.");
            return;
        }

        item.Outcome = stored.Reused ? EventImportImageOutcome.Reused : EventImportImageOutcome.Downloaded;
        item.FailureReason = null;
        item.Detail = stored.Reused ? $"Same image already stored ({stored.ImageId})." : stored.ImageId;
    }

    // "Ticket URL" page -> image address (GS-REQ-009, 9.2.1); records the failure on the item and returns null if none.
    private async Task<string?> FindImageOnPageAsync(EventImportJobItem item, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.PageUrl))
        {
            Fail(item, ImageFetchFailureReason.NoImageOnPage, "Neither Image URL nor Ticket URL is set.");
            return null;
        }

        var page = await _fetcher.FetchPageAsync(item.PageUrl, cancellationToken);
        if (!page.IsSuccess)
        {
            Fail(item, page.FailureReason!.Value, "Ticket URL page: " + page.Detail);
            return null;
        }

        var imageUrl = EventPageImageExtractor.Extract(Encoding.UTF8.GetString(page.Content), page.FinalUri!);
        if (imageUrl == null)
        {
            // Facebook usually serves a login page without these tags (GS-REQ-009, 9.2.2).
            Fail(item, ImageFetchFailureReason.NoImageOnPage, "No og:image, twitter:image or JSON-LD Event image on the page.");
        }

        return imageUrl;
    }

    // A signed Facebook/Instagram CDN address past its "oe" time is reported as expired, not as a plain 403/404.
    private ImageFetchFailureReason ClassifyDownloadFailure(string imageUrl, RemoteFetchResult download)
    {
        var refusedByCdn = download.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound or HttpStatusCode.Gone;
        if (refusedByCdn && Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) && SignedCdnUrl.IsExpired(uri, _timeProvider.GetUtcNow()))
        {
            return ImageFetchFailureReason.Expired;
        }

        return download.FailureReason!.Value;
    }

    private static void Fail(EventImportJobItem item, ImageFetchFailureReason reason, string? detail)
    {
        item.Outcome = EventImportImageOutcome.Failed;
        item.FailureReason = reason;
        item.Detail = Truncate(detail);
    }

    private static string? Truncate(string? value) => value == null || value.Length <= DetailMaxLength ? value : value[..DetailMaxLength];
}
