using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Bilet;

public class BiletPlaceDto
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
