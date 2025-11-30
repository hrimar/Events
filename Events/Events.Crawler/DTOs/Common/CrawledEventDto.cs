namespace Events.Crawler.DTOs.Common;

public class CrawledEventDto
{
    public string ExternalId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }
    public string? SourceUrl { get; set; }
    public decimal? Price { get; set; }
    public bool IsFree { get; set; }
    public Dictionary<string, object> RawData { get; set; } = new();
    public DateTime CrawledAt { get; set; } = DateTime.UtcNow;
}