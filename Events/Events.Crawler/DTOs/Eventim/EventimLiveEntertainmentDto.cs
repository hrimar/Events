using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimLiveEntertainmentDto
{
    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("location")]
    public EventimLocationDto? Location { get; set; }
}
