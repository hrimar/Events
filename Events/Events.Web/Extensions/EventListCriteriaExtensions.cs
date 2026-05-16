using Events.Models.Queries;
using Microsoft.AspNetCore.Routing;

namespace Events.Web.Extensions;

public static class EventListCriteriaExtensions
{
    /// <summary>
    /// Builds route values for link generation. Expects criteria already normalized in the controller.
    /// </summary>
    public static RouteValueDictionary ToRouteValues(this EventListCriteria criteria, int? page = null)
    {
        var route = new RouteValueDictionary
        {
            ["page"] = page ?? criteria.Page,
            ["pageSize"] = criteria.PageSize,
            ["sortBy"] = criteria.SortBy,
            ["sortOrder"] = criteria.SortOrder
        };

        if (!string.IsNullOrWhiteSpace(criteria.Search))
            route["search"] = criteria.Search;

        if (criteria.Status.HasValue)
            route["status"] = (int)criteria.Status.Value;

        if (criteria.CategoryId.HasValue)
            route["categoryId"] = criteria.CategoryId.Value;

        if (criteria.SubCategoryId.HasValue)
            route["subCategoryId"] = criteria.SubCategoryId.Value;

        if (criteria.FromDate.HasValue)
            route["fromDate"] = criteria.FromDate.Value.ToString("yyyy-MM-dd");

        if (criteria.ToDate.HasValue)
            route["toDate"] = criteria.ToDate.Value.ToString("yyyy-MM-dd");

        return route;
    }
}
