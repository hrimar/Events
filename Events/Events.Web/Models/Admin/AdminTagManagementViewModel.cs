using Events.Models.Enums;
using Events.Services.Models.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models.Admin;

public class AdminTagManagementViewModel
{
    public PaginatedList<AdminTagListItemViewModel> Tags { get; set; } = new(new List<AdminTagListItemViewModel>(), 0, 1, AdminTagQuery.DefaultPageSize);
    public string? SearchTerm { get; set; }
    public EventCategory? CategoryFilter { get; set; }
    public bool ShowOrphansOnly { get; set; }
    public bool ShowWithoutCategoryOnly { get; set; }
    public string SortBy { get; set; } = AdminTagSortFields.Name;
    public string SortOrder { get; set; } = AdminTagSortOrders.Asc;
    public AdminTagStatisticsViewModel Statistics { get; set; } = new();
    public List<SelectListItem> CategoryOptions { get; set; } = new();

    public string SortByNormalized => AdminTagSortFields.Normalize(SortBy);
    public string SortOrderNormalized => AdminTagSortOrders.Normalize(SortOrder);

    public bool IsCurrentSort(string column)
    {
        var normalizedColumn = AdminTagSortFields.Normalize(column);
        return string.Equals(SortByNormalized, normalizedColumn, StringComparison.OrdinalIgnoreCase);
    }

    public RouteValueDictionary BuildSortRouteValues(string column)
    {
        var normalizedColumn = AdminTagSortFields.Normalize(column);
        var isCurrent = IsCurrentSort(normalizedColumn);
        var nextOrder = isCurrent && SortOrderNormalized == AdminTagSortOrders.Asc
            ? AdminTagSortOrders.Desc
            : AdminTagSortOrders.Asc;

        return new RouteValueDictionary
        {
            ["page"] = 1,
            ["pageSize"] = Tags.PageSize,
            ["search"] = SearchTerm,
            ["category"] = CategoryFilter.HasValue ? (int?)CategoryFilter.Value : null,
            ["showOrphansOnly"] = ShowOrphansOnly,
            ["showWithoutCategoryOnly"] = ShowWithoutCategoryOnly,
            ["sortBy"] = normalizedColumn,
            ["sortOrder"] = nextOrder
        };
    }
}
