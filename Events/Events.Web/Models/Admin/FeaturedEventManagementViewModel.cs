namespace Events.Web.Models.Admin;

public class FeaturedEventManagementViewModel
{
    public List<AdminEventViewModel> FeaturedEvents { get; set; } = new();
    public PaginatedList<AdminEventViewModel> AvailableEvents { get; set; } = new(new List<AdminEventViewModel>(), 0, 1, 20);
    public int MaxFeaturedEvents { get; set; } = 6;
    public bool CanAddMore => FeaturedEvents.Count < MaxFeaturedEvents;

    public string? SearchTerm { get; set; }
    public string? CategoryFilter { get; set; }
}
