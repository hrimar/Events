using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Paysera;

public class PayseraCategoryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
