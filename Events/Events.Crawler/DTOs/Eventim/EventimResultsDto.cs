using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimResultsDto
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("nextPage")]
    public string? NextPage { get; set; }

    [JsonPropertyName("filters")]
    public EventimFiltersDto? Filters { get; set; }

    [JsonPropertyName("suggestions")]
    public List<string>? Suggestions { get; set; }

    [JsonPropertyName("results")]
    public List<EventimEventDto>? Results { get; set; }
}
