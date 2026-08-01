using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Paysera;

public class PayseraLocationDataDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }

    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }
}
