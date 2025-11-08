using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Infrastructure.Database;

internal sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<User> m_userManager;
    private readonly ApplicationDbContext m_context;
    private readonly IConfiguration m_configuration;
    private readonly ILogger<DatabaseSeeder> m_logger;

    public DatabaseSeeder(
        UserManager<User> userManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        m_userManager = userManager;
        m_context = context;
        m_configuration = configuration;
        m_logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAndPermissionsAsync();
        await SeedDefaultUserAsync();
    }

    private async Task SeedRolesAndPermissionsAsync()
    {
        if (await m_context.Roles.AnyAsync())
        {
            return;
        }

    }

    private async Task SeedDefaultUserAsync()
    {
        if (m_userManager.Users.Any())
        {
            return;
        }

        string? username = m_configuration["DefaultUser:Username"];
        string? email = m_configuration["DefaultUser:Email"];
        string? password = m_configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            m_logger.LogWarning("Default user username or password not configured. Skipping user seeding");

            return;
        }

        User defaultUser = new()
        {
            UserName = username,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            EmailConfirmed = !string.IsNullOrWhiteSpace(email)
        };

        IdentityResult result = await m_userManager.CreateAsync(defaultUser, password);

        if (result.Succeeded)
        {
            defaultUser.Roles = [Role.Admin];

            await m_context.SaveChangesAsync();
        }
        else
        {
            m_logger.LogError("Failed to create default user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
