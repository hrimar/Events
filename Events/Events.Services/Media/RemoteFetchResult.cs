using System.Net;
using Events.Models.Enums;

namespace Events.Services.Media;

// Outcome of a safe download: either the body bytes or the reason it was refused or failed (GS-REQ-009, 9.1, 9.3.3).
public sealed class RemoteFetchResult
{
    public bool IsSuccess => FailureReason == null;

    public byte[] Content { get; private init; } = Array.Empty<byte>();

    // Content-Type header as sent by the server; informative only (the real type is checked by magic bytes).
    public string? ContentType { get; private init; }

    // Address after redirects; the requested address when the request was refused before sending.
    public Uri? FinalUri { get; private init; }

    // Last HTTP status received, if any.
    public HttpStatusCode? StatusCode { get; private init; }

    public ImageFetchFailureReason? FailureReason { get; private init; }

    // Short technical detail for the import report.
    public string? Detail { get; private init; }

    public static RemoteFetchResult Success(byte[] content, string? contentType, Uri finalUri, HttpStatusCode statusCode) =>
        new() { Content = content, ContentType = contentType, FinalUri = finalUri, StatusCode = statusCode };

    public static RemoteFetchResult Failure(ImageFetchFailureReason reason, string detail, Uri? uri = null, HttpStatusCode? statusCode = null) =>
        new() { FailureReason = reason, Detail = detail, FinalUri = uri, StatusCode = statusCode };
}
