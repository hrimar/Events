using System;
using System.Collections.Generic;

namespace Events.Services.Models.Admin;

public class AdminUserQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = AdminUserSortFields.Email;
    public string SortOrder { get; set; } = AdminUserSortOrders.Asc;

    public AdminUserQuery Normalize()
    {
        return new AdminUserQuery
        {
            Page = Page < 1 ? 1 : Page,
            PageSize = PageSize <= 0 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize),
            SearchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
            SortBy = AdminUserSortFields.Normalize(SortBy),
            SortOrder = AdminUserSortOrders.Normalize(SortOrder)
        };
    }
}

public static class AdminUserSortFields
{
    public const string Email = "email";
    public const string Favorites = "favorites";
    public const string Status = "status";
    public const string Registered = "registered";

    public static string Normalize(string? value) => value?.ToLowerInvariant() switch
    {
        Favorites => Favorites,
        Status => Status,
        Registered => Registered,
        "name" => Email,
        _ => Email
    };
}

public static class AdminUserSortOrders
{
    public const string Asc = "asc";
    public const string Desc = "desc";

    public static string Normalize(string? value) => string.Equals(value, Desc, StringComparison.OrdinalIgnoreCase) ? Desc : Asc;
}

public class AdminUserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ConfirmedEmails { get; set; }
    public int LockedUsers { get; set; }
    public int NewUsersThisWeek { get; set; }
}

public class AdminUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int FavoriteEventsCount { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? PreferredCategory { get; set; }
    public IReadOnlyList<string> FavoriteSubCategories { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> FavoriteCategories { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}

public class AdminUserListResult
{
    public IReadOnlyList<AdminUserDto> Users { get; set; } = Array.Empty<AdminUserDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = AdminUserSortFields.Email;
    public string SortOrder { get; set; } = AdminUserSortOrders.Asc;
    public AdminUserStatisticsDto Statistics { get; set; } = new();
}
