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
            _logger.LogInformation("Database already contains users, skipping seeding");
            return;
        }

        string? email = _configuration["DefaultUser:Email"];
        string? password = _configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Default user email or password not configured. Skipping user seeding");
            return;
        }

        User defaultUser = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        IdentityResult result = await _userManager.CreateAsync(defaultUser, password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Default user created successfully: {Email}", email);
        }
        else
        {
            _logger.LogError("Failed to create default user. Errors: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
