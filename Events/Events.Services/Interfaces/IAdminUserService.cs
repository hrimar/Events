using System.Threading;
using System.Threading.Tasks;
using Events.Services.Models.Admin;

namespace Events.Services.Interfaces;

public interface IAdminUserService
{
    Task<AdminUserListResult> GetUsersAsync(AdminUserQuery query, CancellationToken cancellationToken = default);
    Task SetUserRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    Task SetLockoutStateAsync(string userId, bool lockUser, CancellationToken cancellationToken = default);
}
