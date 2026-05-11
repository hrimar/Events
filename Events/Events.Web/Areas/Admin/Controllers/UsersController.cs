using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Events.Web.Models;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly IAdminUserService _adminUserService;

    public UsersController(ILogger<UsersController> logger, IAdminUserService adminUserService)
    {
        _logger = logger;
        _adminUserService = adminUserService;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string sortBy = AdminUserSortFields.Email,
        string sortOrder = AdminUserSortOrders.Asc,
        CancellationToken cancellationToken = default)
    {
        var query = new AdminUserQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = search,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var result = await _adminUserService.GetUsersAsync(query, cancellationToken);

        var items = result.Users
            .Select(u => new AdminUserListItemViewModel
            {
                Id = u.Id,
                DisplayName = string.IsNullOrWhiteSpace(u.DisplayName) ? u.Email : u.DisplayName,
                Email = u.Email,
                FavoriteEventsCount = u.FavoriteEventsCount,
                EmailConfirmed = u.EmailConfirmed,
                IsLockedOut = u.IsLockedOut,
                PreferredCategory = u.PreferredCategory,
                FavoriteCategories = u.FavoriteCategories,
                FavoriteSubCategories = u.FavoriteSubCategories,
                RegisteredAt = u.RegisteredAt,
                Roles = u.Roles
            })
            .ToList();

        var paginatedUsers = new PaginatedList<AdminUserListItemViewModel>(
            items,
            result.TotalCount,
            result.Page,
            result.PageSize);

        var statistics = result.Statistics ?? new AdminUserStatisticsDto();

        var viewModel = new AdminUserManagementViewModel
        {
            Users = paginatedUsers,
            SearchTerm = result.SearchTerm,
            SortBy = result.SortBy,
            SortOrder = result.SortOrder,
            TotalUsers = statistics.TotalUsers,
            ConfirmedEmails = statistics.ConfirmedEmails,
            LockedUsers = statistics.LockedUsers,
            NewUsersThisWeek = statistics.NewUsersThisWeek
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(
        string userId,
        string role,
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string sortBy = AdminUserSortFields.Email,
        string sortOrder = AdminUserSortOrders.Asc)
    {
        await ExecuteAdminActionAsync(
            () => _adminUserService.SetUserRoleAsync(userId, role),
            successMessage: "User role updated successfully.",
            errorMessage: "Unable to update user role.",
            errorLog: "Failed to update role for user {UserId}",
            logArgs: userId);

        return RedirectToAction(nameof(Index), new { page, pageSize, search, sortBy, sortOrder });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(
        string userId,
        bool lockUser,
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string sortBy = AdminUserSortFields.Email,
        string sortOrder = AdminUserSortOrders.Asc)
    {
        await ExecuteAdminActionAsync(
            () => _adminUserService.SetLockoutStateAsync(userId, lockUser),
            successMessage: lockUser ? "User locked successfully." : "User unlocked successfully.",
            errorMessage: "Unable to update user access.",
            errorLog: "Failed to toggle lock for user {UserId}",
            logArgs: userId);

        return RedirectToAction(nameof(Index), new { page, pageSize, search, sortBy, sortOrder });
    }

    private async Task ExecuteAdminActionAsync(
        Func<Task> action,
        string successMessage,
        string errorMessage,
        string errorLog,
        params object[] logArgs)
    {
        try
        {
            await action();
            TempData["SuccessMessage"] = successMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, errorLog, logArgs);
            TempData["ErrorMessage"] = errorMessage;
        }
    }
}
