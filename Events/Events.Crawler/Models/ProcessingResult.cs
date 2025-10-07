namespace Events.Crawler.Models;

public class ProcessingResult
{
    public int EventsProcessed { get; set; }
    public int EventsCreated { get; set; }
    public int EventsUpdated { get; set; }
    public int EventsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<int> ProcessedEventIds { get; set; } = new();
}