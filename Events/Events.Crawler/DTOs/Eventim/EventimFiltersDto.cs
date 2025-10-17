using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimFiltersDto
{
    [JsonPropertyName("country")]
    public List<string>? Country { get; set; }

    [JsonPropertyName("city")]
    public List<string>? City { get; set; }

    [JsonPropertyName("category")]
    public List<int>? Category { get; set; }

    [JsonPropertyName("mainCategory")]
    public List<int>? MainCategory { get; set; }

    [JsonPropertyName("minDate")]
    public EventimDateDto? MinDate { get; set; }

    [JsonPropertyName("maxDate")]
    public EventimDateDto? MaxDate { get; set; }
}