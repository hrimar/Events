using Events.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Events.Data.Services;

public static class DbSeederService
{
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager, logger);
        await SeedUsersAsync(userManager, logger);
    }

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        string[] roles = { "Administrator", "EventManager", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);
                
                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role '{RoleName}': {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogDebug("Role '{RoleName}' already exists", roleName);
            }
        }
    }

    public static async Task SeedUsersAsync(UserManager<User> userManager, ILogger logger)
    {
        logger.LogWarning("Seeding development users with hardcoded passwords - NOT FOR PRODUCTION!");
        
        // Seed Administrator user
        await SeedUserAsync(userManager, logger, 
            email: "admin@events.local", 
            password: "Admin123!", 
            roles: new[] { "Administrator" });

        // Seed Event Manager user
        await SeedUserAsync(userManager, logger, 
            email: "manager@events.local", 
            password: "Manager123!", 
            roles: new[] { "EventManager" });

        // Seed Regular user
        await SeedUserAsync(userManager, logger, 
            email: "user@events.local", 
            password: "User123!", 
            roles: new[] { "User" });
    }

    private static async Task SeedUserAsync(UserManager<User> userManager, ILogger logger, 
        string email, string password, string[] roles)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true // Skip email confirmation for seeded users
            };

            var result = await userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                logger.LogInformation("User '{Email}' created successfully", email);
                
                // Add user to roles
                foreach (var role in roles)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("User '{Email}' added to role '{Role}'", email, role);
                    }
                    else
                    {
                        logger.LogError("Failed to add user '{Email}' to role '{Role}': {Errors}", 
                            email, role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            else
            {
                logger.LogError("Failed to create user '{Email}': {Errors}", 
                    email, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogDebug("User '{Email}' already exists", email);
        }
    }
}