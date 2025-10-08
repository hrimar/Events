using Events.Models.Enums;

namespace Events.Crawler.Models;

public class TaggingResult
{
    public EventCategory? SuggestedCategory { get; set; }
    public List<string> SuggestedTags { get; set; } = new();
    public Dictionary<string, double> Confidence { get; set; } = new();
}