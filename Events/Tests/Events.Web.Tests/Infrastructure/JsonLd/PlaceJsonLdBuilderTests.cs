using Events.Models.Entities;
using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Tests.Infrastructure.JsonLd;

public class PlaceJsonLdBuilderTests
{
    private static CanonicalVenue CreateVenue() => new() { Name = "NDK", NameEn = "NDK", Slug = "ndk", City = "Sofia" };

    [Fact]
    public void BuildPlace_OwnPageUrlGoesToUrl_WebsiteUrlGoesToSameAs()
    {
        var venue = CreateVenue();
        venue.WebsiteUrl = "https://ndk.bg";

        var result = PlaceJsonLdBuilder.BuildPlace(venue, "https://go-sofia.com/venues/ndk");

        Assert.Equal("https://go-sofia.com/venues/ndk", result["url"]);
        Assert.Equal("https://ndk.bg", result["sameAs"]);
    }

    [Fact]
    public void BuildPlace_NoAddress_OmitsAddressBlock()
    {
        var result = PlaceJsonLdBuilder.BuildPlace(CreateVenue(), null);

        Assert.False(result.ContainsKey("address"));
    }

    [Fact]
    public void BuildPlace_AddressPresent_BuildsPostalAddress()
    {
        var venue = CreateVenue();
        venue.Address = "Bulgaria Sq 1";

        var result = PlaceJsonLdBuilder.BuildPlace(venue, null);

        var address = Assert.IsType<Dictionary<string, string>>(result["address"]);
        Assert.Equal("Bulgaria Sq 1", address["streetAddress"]);
        Assert.Equal("Sofia", address["addressLocality"]);
    }

    [Fact]
    public void BuildPlace_NoCoordinates_OmitsGeoBlock()
    {
        var result = PlaceJsonLdBuilder.BuildPlace(CreateVenue(), null);

        Assert.False(result.ContainsKey("geo"));
    }

    [Fact]
    public void BuildPlace_OnlyOneCoordinateSet_OmitsGeoBlock()
    {
        var venue = CreateVenue();
        venue.Latitude = 42.6m; // Longitude left null - a partial coordinate is not a usable one.

        var result = PlaceJsonLdBuilder.BuildPlace(venue, null);

        Assert.False(result.ContainsKey("geo"));
    }

    [Fact]
    public void BuildPlace_BothCoordinatesSet_BuildsGeoCoordinates()
    {
        var venue = CreateVenue();
        venue.Latitude = 42.6977m;
        venue.Longitude = 23.3219m;

        var result = PlaceJsonLdBuilder.BuildPlace(venue, null);

        var geo = Assert.IsType<Dictionary<string, object>>(result["geo"]);
        Assert.Equal(42.6977m, geo["latitude"]);
        Assert.Equal(23.3219m, geo["longitude"]);
    }

    [Fact]
    public void BuildPlace_IncludeContextFalse_OmitsContextKey()
    {
        var result = PlaceJsonLdBuilder.BuildPlace(CreateVenue(), null, includeContext: false);

        Assert.False(result.ContainsKey("@context"));
    }
}
