using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimProductDto
{
    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("url")]
    public EventimUrlDto? Url { get; set; }

    [JsonPropertyName("typeAttributes")]
    public EventimTypeAttributesDto? TypeAttributes { get; set; }

    [JsonPropertyName("rating")]
    public EventimRatingDto? Rating { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("hasRecommendation")]
    public bool HasRecommendation { get; set; }
}
