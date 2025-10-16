namespace Events.Crawler.DTOs.Epaygo;

public class EpaygoResultDto
{
    public List<EpaygoEventDto> Results { get; set; } = new();
}