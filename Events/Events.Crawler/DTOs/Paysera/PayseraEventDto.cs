using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Paysera;

public class PayseraEventDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("category")]
    public PayseraCategoryDto? Category { get; set; }

    [JsonPropertyName("price_from")]
    public string? PriceFrom { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("date_starts")]
    public long? DateStarts { get; set; }

    [JsonPropertyName("date_ends")]
    public long? DateEnds { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("sold_out")]
    public bool SoldOut { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("location_data")]
    public PayseraLocationDataDto? LocationData { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("main_image")]
    public string? MainImage { get; set; }
}
