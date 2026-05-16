using Events.Models.Queries;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models.Admin;

public class AdminEventsIndexViewModel
{
    public PaginatedList<AdminEventViewModel> Events { get; init; } = new(new List<AdminEventViewModel>(), 0, 1, 20);

    public EventListCriteria Filters { get; init; } = new();
    public EventFilterOptions Options { get; init; } = new();
    public string CategoriesJson { get; init; } = "[]";
}

public class EventFilterOptions
{
    public IReadOnlyList<SelectListItem> Categories { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> SubCategories { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Statuses { get; init; } = Array.Empty<SelectListItem>();
}
