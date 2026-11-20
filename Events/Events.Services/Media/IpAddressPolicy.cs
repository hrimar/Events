using System.Net;
using System.Net.Sockets;

namespace Events.Services.Media;

// Decides which addresses the server may download images and pages from, so an imported URL cannot reach
// local, internal or platform services (SSRF protection, GS-REQ-009, 9.1.2).
public static class IpAddressPolicy
{
    // Ranges from the spec, plus a few reserved ranges that are just as unsafe to call from the server.
    private static readonly IpRange[] ForbiddenRanges =
    {
        // From the spec (GS-REQ-009, 9.1.2).
        IpRange.Parse("127.0.0.0/8"),
        IpRange.Parse("10.0.0.0/8"),
        IpRange.Parse("172.16.0.0/12"),
        IpRange.Parse("192.168.0.0/16"),
        IpRange.Parse("169.254.0.0/16"),
        IpRange.Parse("::1/128"),
        IpRange.Parse("fc00::/7"),

        // Stricter than the spec: "this network", carrier-grade NAT, multicast, reserved and broadcast.
        IpRange.Parse("0.0.0.0/8"),
        IpRange.Parse("100.64.0.0/10"),
        IpRange.Parse("224.0.0.0/4"),
        IpRange.Parse("240.0.0.0/4"),

        // Azure platform address (DNS, health and metadata for the host).
        IpRange.Parse("168.63.129.16/32"),

        // Stricter than the spec: unspecified and IPv4-compatible, link-local and multicast IPv6.
        IpRange.Parse("::/96"),
        IpRange.Parse("fe80::/10"),
        IpRange.Parse("ff00::/8")
    };

    public static bool IsForbidden(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        // "::ffff:127.0.0.1" must be judged as 127.0.0.1.
        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        foreach (var range in ForbiddenRanges)
        {
            if (range.Contains(address))
            {
                return true;
            }
        }

        return false;
    }

    // Host names that always point to the machine itself; rejected before any DNS lookup.
    public static bool IsForbiddenHostName(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return true;
        }

        var normalized = host.Trim().TrimEnd('.').ToLowerInvariant();
        return normalized == "localhost" || normalized.EndsWith(".localhost", StringComparison.Ordinal);
    }

    private readonly struct IpRange
    {
        private readonly byte[] _networkBytes;
        private readonly int _prefixLength;
        private readonly AddressFamily _family;

        private IpRange(IPAddress network, int prefixLength)
        {
            _networkBytes = network.GetAddressBytes();
            _prefixLength = prefixLength;
            _family = network.AddressFamily;
        }

        public static IpRange Parse(string cidr)
        {
            var parts = cidr.Split('/');
            return new IpRange(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
        }

        public bool Contains(IPAddress address)
        {
            if (address.AddressFamily != _family)
            {
                return false;
            }

            var bytes = address.GetAddressBytes();
            var fullBytes = _prefixLength / 8;
            for (var i = 0; i < fullBytes; i++)
            {
                if (bytes[i] != _networkBytes[i])
                {
                    return false;
                }
            }

            var remainingBits = _prefixLength % 8;
            if (remainingBits == 0)
            {
                return true;
            }

            var mask = (byte)(0xFF << (8 - remainingBits));
            return (bytes[fullBytes] & mask) == (_networkBytes[fullBytes] & mask);
        }
    }
}
