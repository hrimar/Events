using Events.Models.Enums;

namespace Events.Models.Queries;

// Filter, search, sort and paging options for the public event listings
// ("All events", categories, search results, related events, sitemap).
// A record, so variations of the same query can be built with "with" (e.g. past search matches).
// The admin listing uses EventListCriteria instead.
public sealed record PublicEventQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public EventStatus? Status { get; init; }

    // Matches events in any of these categories (e.g. Business and Conferences for one homepage quick filter).
    // SubCategoryName takes priority over CategoryNames when both are set.
    public IReadOnlyList<string> CategoryNames { get; init; } = Array.Empty<string>();
    public string? SubCategoryName { get; init; }

    public bool? IsFree { get; init; }

    // Calendar-day bounds; ToDate is inclusive (events at any time during that day match).
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    // Matches events having any of these tags.
    public IReadOnlyList<string> TagNames { get; init; } = Array.Empty<string>();

    // Case-insensitive match on name, description and location (database collation).
    public string? Search { get; init; }

    public string? SortBy { get; init; }
    public string SortOrder { get; init; } = "asc";
}
