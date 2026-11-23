using System.Globalization;

namespace Events.Services.Media;

// Facebook and Instagram CDN addresses (fbcdn.net, cdninstagram.com) are signed and carry their expiry in the
// "oe" query parameter as hexadecimal Unix seconds, e.g. "...&oe=6A1B2C3D". After that moment the CDN answers
// 403/404/410, which the import report shows as "expired" (GS-REQ-009, 9.1, 9.3.3).
public static class SignedCdnUrl
{
    private static readonly string[] SignedHosts = { "fbcdn.net", "cdninstagram.com" };

    public static bool IsSignedCdnHost(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        var host = uri.Host.ToLowerInvariant();
        return SignedHosts.Any(h => host == h || host.EndsWith("." + h, StringComparison.Ordinal));
    }

    // Expiry from "oe"; null when the address is not a signed CDN address or has no valid "oe".
    public static DateTimeOffset? GetExpiry(Uri uri)
    {
        if (!IsSignedCdnHost(uri))
        {
            return null;
        }

        foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0 || !pair[..separator].Equals("oe", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (long.TryParse(pair[(separator + 1)..], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var seconds)
                && seconds is > 0 and < 253402300800)
            {
                return DateTimeOffset.FromUnixTimeSeconds(seconds);
            }

            return null;
        }

        return null;
    }

    public static bool IsExpired(Uri uri, DateTimeOffset now) => GetExpiry(uri) is { } expiry && expiry <= now;
}
