using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimParentCategoryDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
