namespace Events.Crawler.DTOs.Ndk;

public class NdkEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Location { get; set; }
    public string? TicketUrl { get; set; }
    public string? SourceUrl { get; set; }
    public bool IsFree { get; set; }
}
