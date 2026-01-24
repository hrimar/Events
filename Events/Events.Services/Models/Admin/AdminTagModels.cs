using Events.Models.Enums;

namespace Events.Services.Models.Admin;

public class AdminTagQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
    public string? SearchTerm { get; set; }
    public EventCategory? Category { get; set; }
    public bool ShowOrphansOnly { get; set; }
    public bool ShowWithoutCategoryOnly { get; set; }
    public string SortBy { get; set; } = AdminTagSortFields.Name;
    public string SortOrder { get; set; } = AdminTagSortOrders.Asc;

    public AdminTagQuery Normalize()
    {
        return new AdminTagQuery
        {
            Page = Page < 1 ? 1 : Page,
            PageSize = PageSize <= 0 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize),
            SearchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
            Category = Category,
            ShowOrphansOnly = ShowOrphansOnly,
            ShowWithoutCategoryOnly = ShowWithoutCategoryOnly,
            SortBy = AdminTagSortFields.Normalize(SortBy),
            SortOrder = AdminTagSortOrders.Normalize(SortOrder)
        };
    }
}

public static class AdminTagSortFields
{
    public const string Name = "name";
    public const string Usage = "usage";
    public const string Created = "created";

    public static string Normalize(string? value) => value?.ToLowerInvariant() switch
    {
        Usage => Usage,
        Created => Created,
        _ => Name
    };
}

public static class AdminTagSortOrders
{
    public const string Asc = "asc";
    public const string Desc = "desc";

    public static string Normalize(string? value) => string.Equals(value, Desc, StringComparison.OrdinalIgnoreCase) ? Desc : Asc;
}
