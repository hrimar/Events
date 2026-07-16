using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Web.Infrastructure.JsonLd;

// Builds a schema.org Event object for a single Event entity. Reused by both the
// Event details page (top-level schema) and VenuesController's nested venue "event"
// array, so the two pages never disagree about how an Event is represented.
public static class EventJsonLdBuilder
{
    private const string DefaultImagePath = "/images/default_event_image.jpeg";
    private const string Currency = "BGN"; // Go Sofia only covers events in Sofia/Bulgaria.

    // includeContext controls whether "@context" is emitted - true when this Event is
    // the root JSON-LD object (event details page), false when nested inside another
    // schema (e.g. VenuesController's "event" array on the venue's own Place schema).
    public static Dictionary<string, object?> BuildEvent(Event eventEntity, string baseUrl, bool includeContext = true)
    {
        var builder = new SafeJsonLdBuilder();

        if (includeContext)
            builder.Add("@context", "https://schema.org");

        builder
            .Add("@type", "Event")
            .Add("name", eventEntity.Name)
            .Add("startDate", EventDateTimeHelper.ToIso8601StartDate(eventEntity.Date, eventEntity.StartTime))
            .Add("eventStatus", MapEventStatus(eventEntity.Status))
            .Add("eventAttendanceMode", "https://schema.org/OfflineEventAttendanceMode")
            .AddIfNotEmpty("description", eventEntity.Description)
            .Add("url", $"{baseUrl}/Events/Details/{eventEntity.Id}")
            .Add("image", BuildAbsoluteImageUrl(eventEntity, baseUrl))
            .Add("location", BuildLocation(eventEntity));

        var offers = BuildOffers(eventEntity);
        if (offers != null)
            builder.Add("offers", offers);

        // performer/organizer intentionally omitted - Event/CanonicalVenue carry no such
        // data today; adding them here would mean fabricating values Google could flag.
        return builder.Build();
    }

    private static Dictionary<string, object?> BuildLocation(Event eventEntity)
    {
        if (eventEntity.CanonicalVenue != null)
            return PlaceJsonLdBuilder.BuildPlace(eventEntity.CanonicalVenue, includeContext: false);

        // No canonical venue matched yet - fall back to the crawled free-text location/city
        // only, rather than fabricating a street address we don't have.
        var fallback = new SafeJsonLdBuilder()
            .Add("@type", "Place")
            .AddIfNotEmpty("name", eventEntity.Location);

        if (!string.IsNullOrWhiteSpace(eventEntity.City))
        {
            fallback.Add("address", new Dictionary<string, string>
            {
                ["@type"] = "PostalAddress",
                ["addressLocality"] = eventEntity.City
            });
        }

        return fallback.Build();
    }

    private static Dictionary<string, object?>? BuildOffers(Event eventEntity)
    {
        // Only emit an Offer when there is a real signal: a known free/paid price, or an
        // actual ticket link - never a guessed price.
        if (!eventEntity.IsFree && !eventEntity.Price.HasValue && string.IsNullOrWhiteSpace(eventEntity.TicketUrl))
            return null;

        var offer = new SafeJsonLdBuilder()
            .Add("@type", "Offer")
            .AddIfNotEmpty("url", eventEntity.TicketUrl);

        if (eventEntity.IsFree)
        {
            offer.Add("price", 0).Add("priceCurrency", Currency);
        }
        else if (eventEntity.Price.HasValue)
        {
            offer.Add("price", eventEntity.Price.Value).Add("priceCurrency", Currency);
        }

        offer.Add("availability", eventEntity.Status == EventStatus.SoldOut
            ? "https://schema.org/SoldOut"
            : "https://schema.org/InStock");

        return offer.Build();
    }

    private static string BuildAbsoluteImageUrl(Event eventEntity, string baseUrl)
    {
        var imagePath = eventEntity.ImageUrl ?? DefaultImagePath;

        return imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? imagePath
            : $"{baseUrl}{imagePath}";
    }

    private static string MapEventStatus(EventStatus status) => status switch
    {
        EventStatus.Cancelled => "https://schema.org/EventCancelled",
        EventStatus.Postponed => "https://schema.org/EventPostponed",
        _ => "https://schema.org/EventScheduled" // Draft/Published/SoldOut - still scheduled to happen.
    };
}
