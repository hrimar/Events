using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimProductGroupDto
{
    [JsonPropertyName("productGroupId")]
    public string? ProductGroupId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("productCount")]
    public int ProductCount { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("url")]
    public EventimUrlDto? Url { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("rating")]
    public EventimRatingDto? Rating { get; set; }

    [JsonPropertyName("categories")]
    public List<EventimCategoryDto>? Categories { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("rankingStatistic")]
    public EventimRankingStatisticDto? RankingStatistic { get; set; }

    [JsonPropertyName("products")]
    public List<EventimProductDto>? Products { get; set; }
}
