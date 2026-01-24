using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events.Data.Context;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class AdminUserService : IAdminUserService
{
    private static readonly string[] AssignableRoles = { "User", "EventManager" };

    private readonly EventsDbContext _dbContext;
    private readonly ILogger<AdminUserService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUserService(
        EventsDbContext dbContext,
        ILogger<AdminUserService> logger,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<AdminUserListResult> GetUsersAsync(AdminUserQuery query, CancellationToken cancellationToken = default)
    {
        var normalized = (query ?? new AdminUserQuery()).Normalize();

        try
        {
            var usersQuery = _dbContext.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(normalized.SearchTerm))
            {
                var pattern = $"%{normalized.SearchTerm}%";
                usersQuery = usersQuery.Where(u =>
                    (u.Email != null && EF.Functions.Like(u.Email, pattern)) ||
                    (u.UserName != null && EF.Functions.Like(u.UserName, pattern)));
            }

            usersQuery = ApplySorting(usersQuery, normalized);

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            var pagedUsers = await usersQuery
                .Skip((normalized.Page - 1) * normalized.PageSize)
                .Take(normalized.PageSize)
                .Select(u => new AdminUserProjection
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    DisplayName = u.Email ?? u.UserName ?? u.Id,
                    EmailConfirmed = u.EmailConfirmed,
                    LockoutEnd = u.LockoutEnd,
                    RegisteredAt = u.RegisteredAt
                })
                .ToListAsync(cancellationToken);

            var favoritesLookup = await GetFavoriteAggregatesAsync(pagedUsers.Select(u => u.Id).ToList(), cancellationToken);
            var rolesLookup = await GetUsersWithRolesAsync(pagedUsers.Select(u => u.Id).ToList(), cancellationToken);

            var now = DateTimeOffset.UtcNow;

            var dtoList = pagedUsers.Select(u =>
            {
                favoritesLookup.TryGetValue(u.Id, out var favoriteAggregate);
                rolesLookup.TryGetValue(u.Id, out var roles);
                return new AdminUserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    EmailConfirmed = u.EmailConfirmed,
                    IsLockedOut = u.LockoutEnd.HasValue && u.LockoutEnd > now,
                    FavoriteEventsCount = favoriteAggregate?.Count ?? 0,
                    PreferredCategory = favoriteAggregate?.PreferredCategory,
                    FavoriteSubCategories = favoriteAggregate?.SubCategories ?? Array.Empty<string>(),
                    FavoriteCategories = favoriteAggregate?.Categories ?? Array.Empty<string>(),
                    RegisteredAt = u.RegisteredAt,
                    Roles = roles ?? Array.Empty<string>()
                };
            }).ToList();

            var statistics = await BuildStatisticsAsync(cancellationToken);

            return new AdminUserListResult
            {
                Users = dtoList,
                Page = normalized.Page,
                PageSize = normalized.PageSize,
                TotalCount = totalCount,
                SearchTerm = normalized.SearchTerm,
                SortBy = normalized.SortBy,
                SortOrder = normalized.SortOrder,
                Statistics = statistics
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load admin users for page {Page}", query?.Page ?? 1);
            throw;
        }
    }

    private IQueryable<User> ApplySorting(IQueryable<User> query, AdminUserQuery options)
    {
        bool descending = options.SortOrder == AdminUserSortOrders.Desc;

        return options.SortBy switch
        {
            AdminUserSortFields.Email => descending
                ? query.OrderByDescending(u => u.Email ?? u.UserName ?? u.Id).ThenBy(u => u.Id)
                : query.OrderBy(u => u.Email ?? u.UserName ?? u.Id).ThenBy(u => u.Id),
            AdminUserSortFields.Favorites => descending
                ? query.OrderByDescending(u => _dbContext.UserFavoriteEvents.Count(f => f.UserId == u.Id)).ThenBy(u => u.Email ?? u.UserName ?? u.Id)
                : query.OrderBy(u => _dbContext.UserFavoriteEvents.Count(f => f.UserId == u.Id)).ThenBy(u => u.Email ?? u.UserName ?? u.Id),
            AdminUserSortFields.Status => descending
                ? query.OrderByDescending(u => u.EmailConfirmed).ThenByDescending(u => u.LockoutEnd).ThenBy(u => u.Email ?? u.UserName ?? u.Id)
                : query.OrderBy(u => u.EmailConfirmed).ThenBy(u => u.LockoutEnd).ThenBy(u => u.Email ?? u.UserName ?? u.Id),
            AdminUserSortFields.Registered => descending
                ? query.OrderByDescending(u => u.RegisteredAt).ThenBy(u => u.Email ?? u.UserName ?? u.Id)
                : query.OrderBy(u => u.RegisteredAt).ThenBy(u => u.Email ?? u.UserName ?? u.Id),
            _ => descending
                ? query.OrderByDescending(u => u.Email ?? u.UserName ?? u.Id).ThenBy(u => u.Id)
                : query.OrderBy(u => u.Email ?? u.UserName ?? u.Id).ThenBy(u => u.Id)
        };
    }

    /// <summary>
    /// Builds an in-memory lookup with favorite counts and preferred categories for the supplied user ids.
    /// </summary>
    private async Task<Dictionary<string, FavoriteAggregate>> GetFavoriteAggregatesAsync(IReadOnlyCollection<string> userIds, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<string, FavoriteAggregate>(StringComparer.Ordinal);
        }

        var favoriteDetails = await _dbContext.UserFavoriteEvents
            .AsNoTracking()
            .Where(f => userIds.Contains(f.UserId))
            .Select(f => new FavoriteDetail
            {
                UserId = f.UserId,
                CategoryName = f.Event != null && f.Event.Category != null ? f.Event.Category.Name : null,
                SubCategoryName = f.Event != null && f.Event.SubCategory != null ? f.Event.SubCategory.Name : null
            })
            .ToListAsync(cancellationToken);

        return favoriteDetails
            .GroupBy(f => f.UserId)
            .ToDictionary(
                g => g.Key,
                g => new FavoriteAggregate(
                    g.Count(),
                    BuildOrderedList(g.Select(d => d.CategoryName)),
                    BuildOrderedList(g.Select(d => d.SubCategoryName))),
                StringComparer.Ordinal);
    }

    private static IReadOnlyList<string> BuildOrderedList(IEnumerable<string?> names)
    {
        return names
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!.Trim())
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Key)
            .ToList();
    }

    private async Task<AdminUserStatisticsDto> BuildStatisticsAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

        var totalUsers = await _dbContext.Users.CountAsync(cancellationToken);
        var confirmedEmails = await _dbContext.Users.CountAsync(u => u.EmailConfirmed, cancellationToken);
        var lockedUsers = await _dbContext.Users.CountAsync(u => u.LockoutEnd != null && u.LockoutEnd > now, cancellationToken);
        var newUsers = await _dbContext.Users.CountAsync(u => u.RegisteredAt >= oneWeekAgo, cancellationToken);

        return new AdminUserStatisticsDto
        {
            TotalUsers = totalUsers,
            ConfirmedEmails = confirmedEmails,
            LockedUsers = lockedUsers,
            NewUsersThisWeek = newUsers
        };
    }

    private async Task<Dictionary<string, IReadOnlyList<string>>> GetUsersWithRolesAsync(IReadOnlyCollection<string> userIds, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        }

        var roles = await _dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(_dbContext.Roles,
                ur => ur.RoleId,
                role => role.Id,
                (ur, role) => new { ur.UserId, role.Name })
            .ToListAsync(cancellationToken);

        return roles
            .GroupBy(r => r.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<string>)g.Select(r => r.Name).OrderBy(name => name).ToList(),
                StringComparer.Ordinal);
    }

    public async Task SetUserRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User id is required", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("Role name is required", nameof(roleName));
        }

        if (!AssignableRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Role '{roleName}' is not managed by this operation.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User '{userId}' was not found.");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            throw new InvalidOperationException($"Role '{roleName}' does not exist.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase)))
        {
            return; // Already in desired role
        }

        var rolesToRemove = currentRoles
            .Where(r => AssignableRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to remove existing roles: " + string.Join(", ", removeResult.Errors.Select(e => e.Description)));
            }
        }

        var addResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addResult.Succeeded)
        {
            throw new InvalidOperationException("Failed to assign role: " + string.Join(", ", addResult.Errors.Select(e => e.Description)));
        }

        _logger.LogInformation("User {UserId} role changed to {Role}", userId, roleName);
    }

    public async Task SetLockoutStateAsync(string userId, bool lockUser, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User id is required", nameof(userId));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User '{userId}' was not found.");
        }

        var lockoutEnd = lockUser ? DateTimeOffset.UtcNow.AddYears(100) : (DateTimeOffset?)null;
        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to update lockout: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (!lockUser)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
        }

        _logger.LogInformation(lockUser ? "User {UserId} locked" : "User {UserId} unlocked", userId);
    }

    private sealed class AdminUserProjection
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    private sealed class FavoriteDetail
    {
        public string UserId { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
    }

    private sealed class FavoriteAggregate
    {
        public FavoriteAggregate(int count, IReadOnlyList<string> categories, IReadOnlyList<string> subCategories)
        {
            Count = count;
            Categories = categories;
            SubCategories = subCategories;
        }

        public int Count { get; }
        public IReadOnlyList<string> Categories { get; }
        public IReadOnlyList<string> SubCategories { get; }
        public string? PreferredCategory => SubCategories.FirstOrDefault() ?? Categories.FirstOrDefault();
    }
}
