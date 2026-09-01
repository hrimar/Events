using Events.Models.Entities;
using Events.Models.Enums;
using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Tests.Infrastructure.JsonLd;

public class EventJsonLdBuilderTests
{
    private const string BaseUrl = "https://go-sofia.com";

    private static Event CreateEvent() => new()
    {
        Id = 1,
        Name = "Concert",
        Date = new DateTime(2026, 6, 1),
        City = "Sofia",
        Location = "NDK"
    };

    [Fact]
    public void BuildEvent_SetsCanonicalUrlToEventDetailsPage()
    {
        var result = EventJsonLdBuilder.BuildEvent(CreateEvent(), BaseUrl);

        Assert.Equal($"{BaseUrl}/Events/Details/1", result["url"]);
    }

    // Image

    [Fact]
    public void BuildEvent_NoImageUrl_FallsBackToDefaultImagePath()
    {
        var result = EventJsonLdBuilder.BuildEvent(CreateEvent(), BaseUrl);

        Assert.Equal($"{BaseUrl}/images/default_event_image.jpeg", result["image"]);
    }

    [Fact]
    public void BuildEvent_RelativeImageUrl_IsMadeAbsoluteWithBaseUrl()
    {
        var eventEntity = CreateEvent();
        eventEntity.ImageUrl = "/images/concert.jpg";

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        Assert.Equal($"{BaseUrl}/images/concert.jpg", result["image"]);
    }

    [Fact]
    public void BuildEvent_AbsoluteImageUrl_IsUsedAsIs()
    {
        var eventEntity = CreateEvent();
        eventEntity.ImageUrl = "https://cdn.example.com/concert.jpg";

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        Assert.Equal("https://cdn.example.com/concert.jpg", result["image"]);
    }

    // Offers

    [Fact]
    public void BuildEvent_NotFreeNoPriceNoTicketUrl_OmitsOffers()
    {
        var result = EventJsonLdBuilder.BuildEvent(CreateEvent(), BaseUrl);

        Assert.False(result.ContainsKey("offers"));
    }

    [Fact]
    public void BuildEvent_IsFree_OffersHasZeroPriceRegardlessOfPriceField()
    {
        var eventEntity = CreateEvent();
        eventEntity.IsFree = true;
        eventEntity.Price = 20; // should be ignored - IsFree takes priority

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        var offers = Assert.IsType<Dictionary<string, object?>>(result["offers"]);
        Assert.Equal(0, offers["price"]);
        Assert.Equal("BGN", offers["priceCurrency"]);
    }

    [Fact]
    public void BuildEvent_HasPrice_OffersReflectsThatPrice()
    {
        var eventEntity = CreateEvent();
        eventEntity.Price = 45.50m;

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        var offers = Assert.IsType<Dictionary<string, object?>>(result["offers"]);
        Assert.Equal(45.50m, offers["price"]);
    }

    [Fact]
    public void BuildEvent_OnlyTicketUrlKnown_StillEmitsOffersWithoutPrice()
    {
        var eventEntity = CreateEvent();
        eventEntity.TicketUrl = "https://tickets.example.com/concert";

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        var offers = Assert.IsType<Dictionary<string, object?>>(result["offers"]);
        Assert.False(offers.ContainsKey("price"));
        Assert.Equal("https://tickets.example.com/concert", offers["url"]);
    }

    [Fact]
    public void BuildEvent_StatusSoldOut_OffersAvailabilityReflectsSoldOut()
    {
        var eventEntity = CreateEvent();
        eventEntity.IsFree = true;
        eventEntity.Status = EventStatus.SoldOut;

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        var offers = Assert.IsType<Dictionary<string, object?>>(result["offers"]);
        Assert.Equal("https://schema.org/SoldOut", offers["availability"]);
    }

    // Event status mapping

    [Theory]
    [InlineData(EventStatus.Cancelled, "https://schema.org/EventCancelled")]
    [InlineData(EventStatus.Postponed, "https://schema.org/EventPostponed")]
    [InlineData(EventStatus.Published, "https://schema.org/EventScheduled")]
    [InlineData(EventStatus.Draft, "https://schema.org/EventScheduled")]
    [InlineData(EventStatus.SoldOut, "https://schema.org/EventScheduled")]
    public void BuildEvent_MapsEventStatusToSchemaOrgEventStatus(EventStatus status, string expected)
    {
        var eventEntity = CreateEvent();
        eventEntity.Status = status;

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        Assert.Equal(expected, result["eventStatus"]);
    }

    // Location

    [Fact]
    public void BuildEvent_HasCanonicalVenue_UsesPlaceJsonLdBuilderForLocation()
    {
        var eventEntity = CreateEvent();
        eventEntity.CanonicalVenue = new CanonicalVenue { Name = "NDK", NameEn = "NDK", Slug = "ndk", City = "Sofia" };

        var result = EventJsonLdBuilder.BuildEvent(eventEntity, BaseUrl);

        var location = Assert.IsType<Dictionary<string, object?>>(result["location"]);
        Assert.Equal("NDK", location["name"]);
        Assert.Equal($"{BaseUrl}/venues/ndk", location["url"]);
    }

    [Fact]
    public void BuildEvent_NoCanonicalVenue_FallsBackToFreeTextLocationAndCity()
    {
        var result = EventJsonLdBuilder.BuildEvent(CreateEvent(), BaseUrl);

        var location = Assert.IsType<Dictionary<string, object?>>(result["location"]);
        Assert.Equal("NDK", location["name"]);
        var address = Assert.IsType<Dictionary<string, string>>(location["address"]);
        Assert.Equal("Sofia", address["addressLocality"]);
    }

    // includeContext

    [Fact]
    public void BuildEvent_IncludeContextFalse_OmitsContextKey()
    {
        var result = EventJsonLdBuilder.BuildEvent(CreateEvent(), BaseUrl, includeContext: false);

        Assert.False(result.ContainsKey("@context"));
    }
}
