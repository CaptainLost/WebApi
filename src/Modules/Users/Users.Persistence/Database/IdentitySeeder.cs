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

        await SeedDefaultUserAsync(userManager, context, configuration, logger);
    }

    private static async Task SeedDefaultUserAsync(
        UserManager<User> userManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger logger)
    {
        if (userManager.Users.Any())
        {
            return;
        }

        string? username = configuration["DefaultUser:Username"];
        string? email = configuration["DefaultUser:Email"];
        string? password = configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Default user username or password not configured. Skipping user seeding");

            return;
        }

        User defaultUser = new User
        {
            UserName = username,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            EmailConfirmed = !string.IsNullOrWhiteSpace(email)
        };

        IdentityResult result = await userManager.CreateAsync(defaultUser, password);

        if (!result.Succeeded)
        {
            logger.LogError("Failed to create default user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        defaultUser.Roles = [Role.Admin];

        await context.SaveChangesAsync();
    }
}
