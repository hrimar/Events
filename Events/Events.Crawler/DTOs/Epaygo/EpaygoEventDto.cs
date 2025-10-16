namespace Events.Crawler.DTOs.Epaygo;

public class EpaygoEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Date { get; set; }
    public decimal? Price { get; set; }
    public string? City { get; set; } = "София";
    public string? Url { get; set; }
    public string? TicketUrl { get; set; }
    public string? SourceUrl { get; set; } = "https://epaygo.bg/";
    public bool IsFree { get; set; } = false;
}