namespace Events.Web.Models.Admin;

public class AdminVenueListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int AliasCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public int TotalEventsCount { get; set; }
}
