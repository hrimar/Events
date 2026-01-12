using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimUrlDto
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("domain")]
    public string? Domain { get; set; }
}
