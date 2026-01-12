namespace Events.Crawler.DTOs.Entase;

public class EntaseEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? SourceUrl { get; set; }
    public List<EntaseSpectacleDto> Spectacles { get; set; } = new();
}
