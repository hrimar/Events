using System.Net;
using System.Net.Sockets;
using Events.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Events.Services.Media;

// Safe HTTP GET for external images and pages from the Excel import (GS-REQ-009, 9.1.1, 9.1.2, 9.2.1):
// https only (port 443), at most 3 redirects followed by hand and checked one by one, 10 seconds for the whole
// download, a byte limit that also holds without Content-Length, and no local, internal or platform addresses.
// The address check runs twice: on the URL before sending, and on the resolved IP inside the connect step
// (CreateHandler), so a host name that resolves, or is re-resolved, to an internal IP is refused as well.
public sealed class SafeHttpFetcher : ISafeHttpFetcher
{
    public const string HttpClientName = "SafeMediaFetcher";
    public const long MaxImageBytes = 10 * 1024 * 1024;
    public const long MaxPageBytes = 2 * 1024 * 1024;
    public const int MaxRedirects = 3;
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    private const int HttpsPort = 443;
    private const int ReadBufferSize = 81920;

    // A regular browser User-Agent: some CDNs refuse requests without one.
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36";

    private const string ImageAccept = "image/webp,image/png,image/jpeg,image/gif;q=0.9,*/*;q=0.5";
    private const string PageAccept = "text/html,application/xhtml+xml;q=0.9,*/*;q=0.5";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SafeHttpFetcher> _logger;

    public SafeHttpFetcher(IHttpClientFactory httpClientFactory, ILogger<SafeHttpFetcher> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<RemoteFetchResult> FetchImageAsync(string url, CancellationToken cancellationToken = default) =>
        FetchAsync(url, MaxImageBytes, ImageAccept, cancellationToken);

    public Task<RemoteFetchResult> FetchPageAsync(string url, CancellationToken cancellationToken = default) =>
        FetchAsync(url, MaxPageBytes, PageAccept, cancellationToken);

    // Primary handler for the named HttpClient (registered in AddSafeHttpFetcher).
    public static SocketsHttpHandler CreateHandler() => new()
    {
        AllowAutoRedirect = false,
        UseCookies = false,
        UseProxy = false,
        AutomaticDecompression = DecompressionMethods.All,
        ConnectTimeout = RequestTimeout,
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        ConnectCallback = ConnectToAllowedAddressAsync
    };

    private async Task<RemoteFetchResult> FetchAsync(string url, long maxBytes, string accept, CancellationToken cancellationToken)
    {
        // On Linux "/path" parses as an absolute file URI (file:///path), so file URIs are rejected here as well,
        // giving the same result on every OS.
        if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) || uri.IsFile)
        {
            return RemoteFetchResult.Failure(ImageFetchFailureReason.InvalidUrl, "Not an absolute web URL.");
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(RequestTimeout);

        var httpClient = _httpClientFactory.CreateClient(HttpClientName);

        try
        {
            for (var redirects = 0; ; redirects++)
            {
                var refusal = CheckTarget(uri);
                if (refusal != null)
                {
                    return LogFailure(refusal);
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
                request.Headers.TryAddWithoutValidation("Accept", accept);

                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);

                if (IsRedirect(response.StatusCode))
                {
                    if (redirects >= MaxRedirects)
                    {
                        return LogFailure(RemoteFetchResult.Failure(ImageFetchFailureReason.HttpError, $"More than {MaxRedirects} redirects.", uri, response.StatusCode));
                    }

                    var location = response.Headers.Location;
                    if (location == null)
                    {
                        return LogFailure(RemoteFetchResult.Failure(ImageFetchFailureReason.HttpError, "Redirect without a Location header.", uri, response.StatusCode));
                    }

                    uri = location.IsAbsoluteUri ? location : new Uri(uri, location);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var reason = response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone
                        ? ImageFetchFailureReason.NotFound
                        : ImageFetchFailureReason.HttpError;
                    return LogFailure(RemoteFetchResult.Failure(reason, $"HTTP {(int)response.StatusCode}.", uri, response.StatusCode));
                }

                if (response.Content.Headers.ContentLength > maxBytes)
                {
                    return LogFailure(RemoteFetchResult.Failure(
                        ImageFetchFailureReason.TooLarge, $"Over {maxBytes / (1024 * 1024)} MB.", uri, response.StatusCode));
                }

                var content = await ReadLimitedAsync(response.Content, maxBytes, timeoutCts.Token);
                if (content == null)
                {
                    return LogFailure(RemoteFetchResult.Failure(
                        ImageFetchFailureReason.TooLarge, $"Over {maxBytes / (1024 * 1024)} MB.", uri, response.StatusCode));
                }

                return RemoteFetchResult.Success(content, response.Content.Headers.ContentType?.MediaType, uri, response.StatusCode);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return LogFailure(RemoteFetchResult.Failure(ImageFetchFailureReason.Timeout, $"No complete response within {RequestTimeout.TotalSeconds:0} seconds.", uri));
        }
        catch (HttpRequestException ex) when (FindForbiddenAddress(ex) != null)
        {
            return LogFailure(RemoteFetchResult.Failure(ImageFetchFailureReason.ForbiddenAddress, FindForbiddenAddress(ex)!.Message, uri));
        }
        catch (HttpRequestException ex)
        {
            return LogFailure(RemoteFetchResult.Failure(ImageFetchFailureReason.HttpError, Truncate(ex.Message), uri));
        }
    }

    // Checks one hop before sending it: scheme, port and literal or well-known local hosts.
    private static RemoteFetchResult? CheckTarget(Uri uri)
    {
        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return RemoteFetchResult.Failure(ImageFetchFailureReason.NotHttps, $"Scheme '{uri.Scheme}' is not allowed.", uri);
        }

        if (uri.Port != HttpsPort)
        {
            return RemoteFetchResult.Failure(ImageFetchFailureReason.ForbiddenAddress, $"Port {uri.Port} is not allowed.", uri);
        }

        if (IpAddressPolicy.IsForbiddenHostName(uri.DnsSafeHost))
        {
            return RemoteFetchResult.Failure(ImageFetchFailureReason.ForbiddenAddress, $"Host '{uri.Host}' is not allowed.", uri);
        }

        if ((uri.HostNameType == UriHostNameType.IPv4 || uri.HostNameType == UriHostNameType.IPv6)
            && IPAddress.TryParse(uri.DnsSafeHost, out var literal)
            && IpAddressPolicy.IsForbidden(literal))
        {
            return RemoteFetchResult.Failure(ImageFetchFailureReason.ForbiddenAddress, $"Address {literal} is not allowed.", uri);
        }

        return null;
    }

    // Resolves the host and connects only if none of its addresses is forbidden (checked after DNS, GS-REQ-009, 9.1.2).
    private static async ValueTask<Stream> ConnectToAllowedAddressAsync(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
    {
        var host = context.DnsEndPoint.Host;
        var addresses = IPAddress.TryParse(host, out var literal) ? new[] { literal } : await Dns.GetHostAddressesAsync(host, cancellationToken);

        if (addresses.Length == 0 || addresses.Any(IpAddressPolicy.IsForbidden))
        {
            throw new ForbiddenAddressException(host);
        }

        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        try
        {
            await socket.ConnectAsync(addresses, context.DnsEndPoint.Port, cancellationToken);
            return new NetworkStream(socket, ownsSocket: true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    // Reads the body up to maxBytes; returns null when it is longer.
    private static async Task<byte[]?> ReadLimitedAsync(HttpContent content, long maxBytes, CancellationToken cancellationToken)
    {
        await using var stream = await content.ReadAsStreamAsync(cancellationToken);
        using var buffer = new MemoryStream();
        var chunk = new byte[ReadBufferSize];

        int read;
        while ((read = await stream.ReadAsync(chunk, cancellationToken)) > 0)
        {
            if (buffer.Length + read > maxBytes)
            {
                return null;
            }

            buffer.Write(chunk, 0, read);
        }

        return buffer.ToArray();
    }

    private static bool IsRedirect(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.MovedPermanently or HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

    private static ForbiddenAddressException? FindForbiddenAddress(Exception ex)
    {
        for (var current = ex.InnerException; current != null; current = current.InnerException)
        {
            if (current is ForbiddenAddressException forbidden)
            {
                return forbidden;
            }
        }

        return null;
    }

    // Logs host and path only: query strings of CDN addresses carry signatures (fbcdn "oh"/"oe").
    private RemoteFetchResult LogFailure(RemoteFetchResult result)
    {
        _logger.LogInformation("External fetch refused or failed for {Address}: {Reason} ({Detail})",
            result.FinalUri?.GetLeftPart(UriPartial.Path), result.FailureReason, result.Detail);
        return result;
    }

    private static string Truncate(string value) => value.Length <= 300 ? value : value[..300];
}
