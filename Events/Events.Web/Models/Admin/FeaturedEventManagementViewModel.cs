namespace Events.Web.Models.Admin;

public class FeaturedEventManagementViewModel
{
    public List<AdminEventViewModel> FeaturedEvents { get; set; } = new();
    public PaginatedList<AdminEventViewModel> AvailableEvents { get; set; } = new(new List<AdminEventViewModel>(), 0, 1, 20);

    // No limit on featured events (owner's decision of 26.10.2026): 5 go to the homepage carousel, all others to "Don't miss".

    public string? SearchTerm { get; set; }
    public string? CategoryFilter { get; set; }
}
