using Core.Domain.Entities;
using Core.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Users.Persistence.Database;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        ILogger<ApplicationDbContext> logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        await SeedUsersAsync(userManager, context, configuration, logger);
    }

    private static async Task SeedUsersAsync(
        UserManager<User> userManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger logger)
    {
        if (userManager.Users.Any())
        {
            return;
        }

        string? password = configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Default user password not configured. Skipping user seeding");
            return;
        }

        await SeedAdminUserAsync(userManager, context, configuration, password, logger);
        await SeedRegularUsersAsync(userManager, context, password, logger);
    }

    private static async Task SeedAdminUserAsync(
        UserManager<User> userManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        string password,
        ILogger logger)
    {
        string? username = configuration["DefaultUser:Username"];
        string? email = configuration["DefaultUser:Email"];

        if (string.IsNullOrWhiteSpace(username))
        {
            username = "admin";
        }

        User adminUser = new User
        {
            UserName = username,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            EmailConfirmed = !string.IsNullOrWhiteSpace(email)
        };

        IdentityResult result = await userManager.CreateAsync(adminUser, password);

        if (!result.Succeeded)
        {
            logger.LogError("Failed to create admin user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
                
            return;
        }

        adminUser.AssignRole(Role.Admin);
        
        await context.SaveChangesAsync();

        logger.LogInformation("Admin user '{Username}' created successfully", username);
    }

    private static async Task SeedRegularUsersAsync(
        UserManager<User> userManager,
        ApplicationDbContext context,
        string password,
        ILogger logger)
    {
        const int userCount = 100;

        for (int i = 1; i <= userCount; i++)
        {
            string username = $"user{i}";
            string email = $"user{i}@example.com";

            User regularUser = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            IdentityResult result = await userManager.CreateAsync(regularUser, password);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create user '{Username}'. Errors: {Errors}",
                    username,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            regularUser.AssignRole(Role.User);
        }

        await context.SaveChangesAsync();
    }
}
