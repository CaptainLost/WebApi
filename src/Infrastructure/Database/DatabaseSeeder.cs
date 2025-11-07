using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

internal sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<User> m_userManager;
    private readonly IConfiguration m_configuration;
    private readonly ILogger<DatabaseSeeder> m_logger;

    public DatabaseSeeder(
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        m_userManager = userManager;
        m_configuration = configuration;
        m_logger = logger;
    }

    public async Task SeedAsync()
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
            m_logger.LogInformation("Default user created successfully: {Username}", username);
        }
        else
        {
            m_logger.LogError("Failed to create default user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
