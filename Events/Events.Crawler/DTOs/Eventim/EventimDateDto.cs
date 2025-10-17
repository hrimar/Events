using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimDateDto
{
    [JsonPropertyName("dateTime")]
    public string? DateTime { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}