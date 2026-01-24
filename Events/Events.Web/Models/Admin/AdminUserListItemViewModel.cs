using System;
using System.Collections.Generic;

namespace Events.Web.Models.Admin;

public class AdminUserListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int FavoriteEventsCount { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsLockedOut { get; set; }
    public string? PreferredCategory { get; set; }
    public IReadOnlyList<string> FavoriteCategories { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> FavoriteSubCategories { get; set; } = Array.Empty<string>();
    public DateTime RegisteredAt { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
