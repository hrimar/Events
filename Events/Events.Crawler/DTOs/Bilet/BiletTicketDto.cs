using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Bilet;

public class BiletTicketDto
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("data")]
    public List<BiletEventDto> Events { get; set; } = new();

    [JsonPropertyName("next_page_url")]
    public string? NextPageUrl { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}