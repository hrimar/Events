using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimEventDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("itemType")]
    public string? ItemType { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("events")]
    public List<EventimEventInstanceDto>? Events { get; set; }
}
