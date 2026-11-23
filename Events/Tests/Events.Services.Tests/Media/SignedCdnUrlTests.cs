using Events.Services.Media;

namespace Events.Services.Tests.Media;

public class SignedCdnUrlTests
{
    // 0x6A1B2C3D = 1780165693 = 2026-05-30 18:28:13 UTC.
    private static readonly DateTimeOffset Expiry = DateTimeOffset.FromUnixTimeSeconds(0x6A1B2C3D);

    [Theory]
    [InlineData("https://scontent.fsof8-1.fna.fbcdn.net/v/t39.30808-6/photo.jpg?stp=dst-jpg&_nc_cat=1&oh=00_AYB&oe=6A1B2C3D")]
    [InlineData("https://fbcdn.net/photo.jpg?OE=6a1b2c3d")]
    [InlineData("https://scontent.cdninstagram.com/v/photo.jpg?oe=6A1B2C3D&oh=x")]
    public void GetExpiry_SignedCdnAddress_ReadsHexUnixSeconds(string url)
    {
        Assert.Equal(Expiry, SignedCdnUrl.GetExpiry(new Uri(url)));
    }

    [Theory]
    [InlineData("https://cdn.example.bg/photo.jpg?oe=6A1B2C3D")]
    [InlineData("https://notfbcdn.net/photo.jpg?oe=6A1B2C3D")]
    [InlineData("https://scontent.fbcdn.net/photo.jpg")]
    [InlineData("https://scontent.fbcdn.net/photo.jpg?oe=zzzz")]
    [InlineData("https://scontent.fbcdn.net/photo.jpg?toe=6A1B2C3D")]
    public void GetExpiry_NotSignedOrNoValidOe_ReturnsNull(string url)
    {
        Assert.Null(SignedCdnUrl.GetExpiry(new Uri(url)));
    }

    [Fact]
    public void IsExpired_ComparesWithNow()
    {
        var uri = new Uri("https://scontent.fbcdn.net/photo.jpg?oe=6A1B2C3D");

        Assert.False(SignedCdnUrl.IsExpired(uri, Expiry.AddSeconds(-1)));
        Assert.True(SignedCdnUrl.IsExpired(uri, Expiry));
        Assert.True(SignedCdnUrl.IsExpired(uri, Expiry.AddDays(1)));
        Assert.False(SignedCdnUrl.IsExpired(new Uri("https://cdn.example.bg/photo.jpg"), Expiry.AddDays(1)));
    }
}
