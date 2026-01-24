using Events.Web.Models;

namespace Events.Web.Models.Admin;

public class AdminUserManagementViewModel
{
    public PaginatedList<AdminUserListItemViewModel> Users { get; set; } =
        new PaginatedList<AdminUserListItemViewModel>(new List<AdminUserListItemViewModel>(), 0, 1, 20);

    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "email";
    public string SortOrder { get; set; } = "asc";

    public int TotalUsers { get; set; }
    public int ConfirmedEmails { get; set; }
    public int LockedUsers { get; set; }
    public int NewUsersThisWeek { get; set; }
}
