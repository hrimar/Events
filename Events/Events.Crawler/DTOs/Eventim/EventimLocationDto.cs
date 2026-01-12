using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimLocationDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("geoLocation")]
    public EventimGeoLocationDto? GeoLocation { get; set; }
}
