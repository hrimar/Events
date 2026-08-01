using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Paysera;

public class PayseraEventsResponseDto
{
    [JsonPropertyName("items")]
    public List<PayseraEventDto> Items { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }
}
