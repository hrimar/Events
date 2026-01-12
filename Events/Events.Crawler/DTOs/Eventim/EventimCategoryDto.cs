using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimCategoryDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("parentCategory")]
    public EventimParentCategoryDto? ParentCategory { get; set; }
}
