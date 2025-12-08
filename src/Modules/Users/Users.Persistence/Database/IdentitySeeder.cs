using Core.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Users.Application.Abstractions;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.Persistence.Database;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        IUserRepository userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        IPasswordHashingService passwordHashingService = serviceProvider.GetRequiredService<IPasswordHashingService>();
        UsersDbContext dbContext = serviceProvider.GetRequiredService<UsersDbContext>();

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<UsersDbContext>>();

        await SeedAdminUserAsync(userRepository, passwordHashingService, dbContext, configuration, logger);
    }

    private static async Task SeedAdminUserAsync(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        UsersDbContext dbContext,
        IConfiguration configuration,
        ILogger<UsersDbContext> logger)
    {
        var username = configuration["DefaultUser:Username"];
        var email = configuration["DefaultUser:Email"];
        var password = configuration["DefaultUser:Password"];

        if (string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Default user password not configured. Skipping user seeding");
            return;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            username = "admin";
        }

        var emailValue = string.IsNullOrWhiteSpace(email) ? $"{username}@localhost.com" : email;

        Result<User> createUserResult = await CreateUser(username, emailValue, password, userRepository, passwordHashingService);
        if (createUserResult.IsFailure)
        {
            logger.LogError("Failed to create admin user: {Error}", createUserResult.Error.Description);
            return;
        }

        // Fetch the existing role from the database instead of using the static instance
        Role? defaultRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == Role.DefaultUserRole.Id);

        if (defaultRole is null)
        {
            logger.LogError("Default role not found in database. Cannot assign role to admin user");
            return;
        }

        createUserResult.Value.AssignRole(defaultRole);

        userRepository.Add(createUserResult.Value);

        await userRepository.SaveChangesAsync();

        logger.LogInformation("Admin user '{Username}' created successfully", username);
    }

    private static async Task<Result<User>> CreateUser(
        string username, string email, string password,
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        CancellationToken cancellationToken = default)
    {
        Result<Username> usernameResult = Username.Create(username);
        if (usernameResult.IsFailure)
        {
            return Result.Failure<User>(usernameResult.Error);
        }

        Result<Email> emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<User>(emailResult.Error);
        }

        Result<Nickname> nicknameResult = Nickname.Create(username);
        if (nicknameResult.IsFailure)
        {
            return Result.Failure<User>(nicknameResult.Error);
        }

        bool isUsernameUnique = await userRepository.IsUsernameUniqueAsync(usernameResult.Value, cancellationToken);
        if (!isUsernameUnique)
        {
            return Result.Failure<User>(UserErrors.UsernameAlreadyTaken(username));
        }

        bool isEmailUnique = await userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken);
        if (!isEmailUnique)
        {
            return Result.Failure<User>(UserErrors.EmailAlreadyTaken(email));
        }

        string passwordHash = passwordHashingService.HashPassword(password, out byte[] passwordSalt);

        Result<User> createUserResult = User.Create(Guid.NewGuid(), usernameResult.Value, emailResult.Value, passwordHash, passwordSalt, nicknameResult.Value);
        if (createUserResult.IsFailure)
        {
            return Result.Failure<User>(createUserResult.Error);
        }

        return createUserResult;
    }
}
