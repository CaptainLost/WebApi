using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

internal sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (_userManager.Users.Any())
        {
            return;
        }

        string? username = _configuration["DefaultUser:Username"];
        string? email = _configuration["DefaultUser:Email"];
        string? password = _configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Default user username or password not configured. Skipping user seeding");

            return;
        }

        User defaultUser = new()
        {
            UserName = username,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            EmailConfirmed = !string.IsNullOrWhiteSpace(email)
        };

        IdentityResult result = await _userManager.CreateAsync(defaultUser, password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Default user created successfully: {Username}", username);
        }
        else
        {
            _logger.LogError("Failed to create default user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
