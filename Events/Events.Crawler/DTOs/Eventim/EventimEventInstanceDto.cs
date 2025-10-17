using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimEventInstanceDto
{
    [JsonPropertyName("dateStart")]
    public EventimDateDto? DateStart { get; set; }

    [JsonPropertyName("dateEnd")]
    public EventimDateDto? DateEnd { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("lkz")]
    public string? Lkz { get; set; }

    [JsonPropertyName("category")]
    public object? Category { get; set; } // Can be string or int
}