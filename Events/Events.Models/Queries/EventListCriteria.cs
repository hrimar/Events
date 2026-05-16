using Events.Models.Enums;

namespace Events.Models.Queries;

public sealed class EventListCriteria
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public EventStatus? Status { get; init; }
    public int? CategoryId { get; init; }
    public int? SubCategoryId { get; init; }
    public bool? IsFree { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string SortBy { get; init; } = "date";
    public string SortOrder { get; init; } = "asc";

    /// <summary>
    /// Sanitizes inbound query parameters. Call once at the controller boundary before passing downstream.
    /// </summary>
    public EventListCriteria Normalize()
    {
        var page = Page < 1 ? 1 : Page;
        var pageSize = PageSize < 1 ? 20 : Math.Min(PageSize, 50000);
        var sortBy = string.IsNullOrWhiteSpace(SortBy) ? "date" : SortBy.Trim().ToLowerInvariant();
        var sortOrder = string.Equals(SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        return new EventListCriteria
        {
            Page = page,
            PageSize = pageSize,
            Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim(),
            Status = Status,
            CategoryId = CategoryId is > 0 ? CategoryId : null,
            SubCategoryId = SubCategoryId is > 0 ? SubCategoryId : null,
            IsFree = IsFree,
            FromDate = FromDate?.Date,
            ToDate = ToDate?.Date,
            SortBy = sortBy,
            SortOrder = sortOrder
        };
    }

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(Search) ||
        Status.HasValue ||
        CategoryId.HasValue ||
        SubCategoryId.HasValue ||
        IsFree.HasValue ||
        FromDate.HasValue ||
        ToDate.HasValue;
}
