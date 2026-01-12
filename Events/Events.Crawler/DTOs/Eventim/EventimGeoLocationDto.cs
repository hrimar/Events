using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimGeoLocationDto
{
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }
}
