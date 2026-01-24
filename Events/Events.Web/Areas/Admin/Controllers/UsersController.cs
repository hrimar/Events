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
        string sortOrder = AdminUserSortOrders.Asc)
    {
        try
        {
            var query = new AdminUserQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = search,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var result = await _adminUserService.GetUsersAsync(query);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin users list");
            return View(new AdminUserManagementViewModel());
        }
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
        try
        {
            await _adminUserService.SetUserRoleAsync(userId, role);
            TempData["SuccessMessage"] = "User role updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update role for user {UserId}", userId);
            TempData["ErrorMessage"] = "Unable to update user role.";
        }

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
        try
        {
            await _adminUserService.SetLockoutStateAsync(userId, lockUser);
            TempData["SuccessMessage"] = lockUser ? "User locked successfully." : "User unlocked successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle lock for user {UserId}", userId);
            TempData["ErrorMessage"] = "Unable to update user access.";
        }

        return RedirectToAction(nameof(Index), new { page, pageSize, search, sortBy, sortOrder });
    }
}
