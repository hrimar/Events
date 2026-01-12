using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public class EventimTypeAttributesDto
{
    [JsonPropertyName("liveEntertainment")]
    public EventimLiveEntertainmentDto? LiveEntertainment { get; set; }
}
