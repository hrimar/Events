namespace Events.Services.Media;

// Thrown from the connect step when a host resolves to a forbidden address (GS-REQ-009, 9.1.2).
// HttpClient wraps it in an HttpRequestException; SafeHttpFetcher maps it to ImageFetchFailureReason.ForbiddenAddress.
public sealed class ForbiddenAddressException : Exception
{
    public ForbiddenAddressException(string host) : base($"Host '{host}' resolves to a forbidden address.")
    {
    }
}
