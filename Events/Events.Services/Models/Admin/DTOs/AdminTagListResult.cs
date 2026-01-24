using Events.Models.Enums;

namespace Events.Services.Models.Admin.DTOs;

public class AdminTagListResult
{
    public IReadOnlyList<AdminTagDto> Tags { get; set; } = Array.Empty<AdminTagDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string? SearchTerm { get; set; }
    public EventCategory? Category { get; set; }
    public bool ShowOrphansOnly { get; set; }
    public bool ShowWithoutCategoryOnly { get; set; }
    public string SortBy { get; set; } = AdminTagSortFields.Name;
    public string SortOrder { get; set; } = AdminTagSortOrders.Asc;
    public AdminTagStatisticsDto Statistics { get; set; } = new();
}
