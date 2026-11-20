namespace Events.Services.Media;

// Downloads external images and pages for the Excel import with SSRF, size and time limits (GS-REQ-009, 9.1.1, 9.1.2).
public interface ISafeHttpFetcher
{
    // Image body, up to 10 MB (GS-REQ-009, 9.1.1).
    Task<RemoteFetchResult> FetchImageAsync(string url, CancellationToken cancellationToken = default);

    // HTML page body, up to 2 MB (GS-REQ-009, 9.2.1).
    Task<RemoteFetchResult> FetchPageAsync(string url, CancellationToken cancellationToken = default);
}
