using System.Text.Json.Serialization;

namespace Events.Crawler.DTOs.Eventim;

public class EventimProductGroupsResponseDto
{
    [JsonPropertyName("productGroups")]
    public List<EventimProductGroupDto>? ProductGroups { get; set; }
}
