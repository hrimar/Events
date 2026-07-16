using Events.Models.Entities;

namespace Events.Web.Infrastructure.JsonLd;

// Builds a schema.org Place object for a CanonicalVenue. Reused both as the top-level
// schema on the venue details page and, nested as an Event's "location", from the
// Event JSON-LD builder - so venue address/geo logic only lives in one place.
public static class PlaceJsonLdBuilder
{
    // includeContext controls whether "@context" is emitted - true when Place is the
    // root JSON-LD object (venue page), false when it is nested inside another schema
    // (e.g. as an Event's "location").
    public static Dictionary<string, object?> BuildPlace(CanonicalVenue venue, bool includeContext = true)
    {
        var builder = new SafeJsonLdBuilder();

        if (includeContext)
            builder.Add("@context", "https://schema.org");

        builder.Add("@type", "Place");
        builder.Add("name", venue.Name);
        builder.AddIfNotEmpty("description", venue.Description);
        builder.AddIfNotEmpty("url", venue.WebsiteUrl);

        if (!string.IsNullOrEmpty(venue.Address))
        {
            builder.Add("address", new Dictionary<string, string>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = venue.Address,
                ["addressLocality"] = venue.City
            });
        }

        if (venue.Latitude.HasValue && venue.Longitude.HasValue)
        {
            builder.Add("geo", new Dictionary<string, object>
            {
                ["@type"] = "GeoCoordinates",
                ["latitude"] = venue.Latitude.Value,
                ["longitude"] = venue.Longitude.Value
            });
        }

        return builder.Build();
    }
}
