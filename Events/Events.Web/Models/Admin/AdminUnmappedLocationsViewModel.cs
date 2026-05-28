using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models.Admin;

public class AdminUnmappedLocationsViewModel
{
    public IReadOnlyList<AdminUnmappedLocationItemViewModel> Items { get; set; } =
        Array.Empty<AdminUnmappedLocationItemViewModel>();

    // Dropdown options for selecting a canonical venue to map to
    public IEnumerable<SelectListItem> VenueOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public int TotalUnmappedEvents { get; set; }
}

public class AdminUnmappedLocationItemViewModel
{
    public string Location { get; set; } = string.Empty;
    public int EventCount { get; set; }
}
