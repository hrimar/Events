using System.Net;
using Events.Services.Media;

namespace Events.Services.Tests.Media;

public class IpAddressPolicyTests
{
    [Theory]
    // From the spec (GS-REQ-009, 9.1.2).
    [InlineData("127.0.0.1")]
    [InlineData("127.255.255.254")]
    [InlineData("10.0.0.1")]
    [InlineData("10.255.255.255")]
    [InlineData("172.16.0.1")]
    [InlineData("172.31.255.255")]
    [InlineData("192.168.1.10")]
    [InlineData("169.254.169.254")]
    [InlineData("::1")]
    [InlineData("fc00::1")]
    [InlineData("fdff:ffff::1")]
    // Stricter than the spec.
    [InlineData("0.0.0.0")]
    [InlineData("100.64.0.1")]
    [InlineData("168.63.129.16")]
    [InlineData("224.0.0.1")]
    [InlineData("255.255.255.255")]
    [InlineData("::")]
    [InlineData("fe80::1")]
    [InlineData("ff02::1")]
    // IPv4-mapped IPv6 is judged as IPv4.
    [InlineData("::ffff:127.0.0.1")]
    [InlineData("::ffff:10.1.2.3")]
    public void IsForbidden_InternalOrReservedAddress_ReturnsTrue(string address)
    {
        Assert.True(IpAddressPolicy.IsForbidden(IPAddress.Parse(address)));
    }

    [Theory]
    [InlineData("8.8.8.8")]
    [InlineData("157.240.1.35")]
    [InlineData("172.15.255.255")]
    [InlineData("172.32.0.1")]
    [InlineData("192.169.0.1")]
    [InlineData("11.0.0.1")]
    [InlineData("100.128.0.1")]
    [InlineData("2a03:2880:f12f::1")]
    [InlineData("::ffff:8.8.8.8")]
    public void IsForbidden_PublicAddress_ReturnsFalse(string address)
    {
        Assert.False(IpAddressPolicy.IsForbidden(IPAddress.Parse(address)));
    }

    [Theory]
    [InlineData("localhost")]
    [InlineData("LOCALHOST.")]
    [InlineData("api.localhost")]
    [InlineData("")]
    public void IsForbiddenHostName_LocalHostName_ReturnsTrue(string host)
    {
        Assert.True(IpAddressPolicy.IsForbiddenHostName(host));
    }

    [Theory]
    [InlineData("scontent.fsof8-1.fna.fbcdn.net")]
    [InlineData("www.facebook.com")]
    [InlineData("notlocalhost.com")]
    public void IsForbiddenHostName_PublicHostName_ReturnsFalse(string host)
    {
        Assert.False(IpAddressPolicy.IsForbiddenHostName(host));
    }
}
