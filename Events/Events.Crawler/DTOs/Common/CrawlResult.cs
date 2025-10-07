namespace Events.Crawler.DTOs.Common;

public class CrawlResult
{
    public string Source { get; set; } = string.Empty;
    public DateTime CrawledAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int EventsFound { get; set; }
    public int EventsProcessed { get; set; }
    public int EventsSkipped { get; set; }
    public TimeSpan Duration { get; set; }
    public List<CrawledEventDto> Events { get; set; } = new();
}