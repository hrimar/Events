namespace Events.Crawler.DTOs.Kupibileti;

// Show-level data read from a single show detail page (itemprop="name"/"startDate"/"location").
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
