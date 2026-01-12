using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimRatingDto
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("average")]
    public double Average { get; set; }
}
