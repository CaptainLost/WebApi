using Core.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Users.Application.Abstractions;
using Users.Domain.Configuration;
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

        var defaultUserSettings = serviceProvider.GetRequiredService<IOptions<AdminUserSettings>>();
        var logger = serviceProvider.GetRequiredService<ILogger<UsersDbContext>>();

        await SeedAdminUserAsync(userRepository, passwordHashingService, dbContext, defaultUserSettings.Value, logger);
    }

    private static async Task SeedAdminUserAsync(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        UsersDbContext dbContext,
        AdminUserSettings defaultUserSettings,
        ILogger<UsersDbContext> logger)
    {
        var username = defaultUserSettings.Username;
        var email = defaultUserSettings.Email;
        var password = defaultUserSettings.Password;

        Result<User> createUserResult = await CreateUser(username, email, password, userRepository, passwordHashingService);
        if (createUserResult.IsFailure)
        {
            logger.LogError("Failed to create admin user: {Error}", createUserResult.Error.Description);
            return;
        }

        Role? defaultRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == Role.DefaultUserRoleName);

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
