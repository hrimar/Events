namespace Events.Models.DTOs;

public class VenueWithStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? PhotoUrl { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int AliasCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public int TotalEventsCount { get; set; }
}
