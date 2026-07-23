namespace Events.Crawler.Models;

public class ProcessingResult
{
    public int EventsProcessed { get; set; }
    public int EventsCreated { get; set; }
    public int EventsUpdated { get; set; }
    public int EventsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<int> ProcessedEventIds { get; set; } = new();

    // Per-event outcome tagged with its crawler Source, so callers can aggregate
    // Created/Updated/Skipped by source without changing how events are batched.
    public List<EventOutcome> Outcomes { get; set; } = new();
}
public enum EventOutcomeType
{
    Created,
    Updated,
    Skipped
}

public record EventOutcome(string Source, EventOutcomeType Type);