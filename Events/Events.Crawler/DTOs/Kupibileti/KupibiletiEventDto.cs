namespace Events.Crawler.DTOs.Kupibileti;

// Card-level data read from the list page, before navigating to the detail page.
public class KupibiletiEventDto
{
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? DetailUrl { get; set; }
    public string? SourceUrl { get; set; }
}
