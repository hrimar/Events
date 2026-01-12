namespace Events.Crawler.DTOs.Eventim;

public class EventimEventInstanceDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Date { get; set; }
    public decimal? Price { get; set; }
    public string? City { get; set; }
    public string? TicketUrl { get; set; }
    public string? SourceUrl { get; set; }
    public bool IsFree { get; set; }
    public string? Venue { get; set; }
}
