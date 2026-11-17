namespace Events.Crawler.DTOs.Kupibileti;

// Show-level data read from a single show detail page's schema.org "Event" JSON-LD block.
public class KupibiletiShowDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public string? TicketUrl { get; set; }
    public string? City { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
}
