using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimRankingStatisticDto
{
    [JsonPropertyName("recommendationPriority")]
    public int RecommendationPriority { get; set; }
}
