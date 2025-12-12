using Core.Domain.Messaging;
using Microsoft.Extensions.Options;
using Users.Application.Abstractions;
using Users.Domain.Configuration;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Authentication;

internal sealed class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserSettings _userSettings;

    public AccountService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenService jwtTokenService,
        IOptions<UserSettings> userSettings)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenService = jwtTokenService;
        _userSettings = userSettings.Value;
    }

    public async Task<Result<string>> LoginAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        if (user.IsLockedOut())
        {
            return Result.Failure<string>(UserErrors.AccountLockedOut);
        }

        bool isPasswordValid = _passwordHashingService.VerifyPassword(password, user.Password.Hash, user.Password.Salt);

        if (!isPasswordValid)
        {
            user.RecordFailedLogin(_userSettings);
            await _userRepository.SaveChangesAsync();

            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.ResetFailedLoginAttempts();
            await _userRepository.SaveChangesAsync();
        }

        string token = _jwtTokenService.GenerateToken(user);

        return Result.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        Result<Username> usernameResult = Username.Create(username);
        if (usernameResult.IsFailure)
        {
            return Result.Failure<string>(usernameResult.Error);
        }

        Result<Email> emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<string>(emailResult.Error);
        }

        Result<Nickname> nicknameResult = Nickname.Create(username);
        if (nicknameResult.IsFailure)
        {
            return Result.Failure<string>(nicknameResult.Error);
        }

        bool isUsernameUnique = await _userRepository.IsUsernameUniqueAsync(usernameResult.Value, cancellationToken);
        if (!isUsernameUnique)
        {
            return Result.Failure<string>(UserErrors.UsernameAlreadyTaken(username));
        }

        bool isEmailUnique = await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken);
        if (!isEmailUnique)
        {
            return Result.Failure<string>(UserErrors.EmailAlreadyTaken(email));
        }

        Result<PlainPassword> plainPasswordResult = PlainPassword.Create(password);
        if (plainPasswordResult.IsFailure)
        {
            return Result.Failure<string>(plainPasswordResult.Error);
        }

        string passwordHash = _passwordHashingService.HashPassword(plainPasswordResult.Value.Value, out byte[] passwordSalt);

        Result<Password> passwordResult = Password.Create(passwordHash, passwordSalt);
        if (passwordResult.IsFailure)
        {
            return Result.Failure<string>(UserErrors.RegistrationFailed);
        }

        Result<User> createUserResult = User.Create(Guid.NewGuid(), usernameResult.Value, emailResult.Value, passwordResult.Value, nicknameResult.Value);
        if (createUserResult.IsFailure)
        {
            return Result.Failure<string>(createUserResult.Error);
        }

        User newUser = createUserResult.Value;

        Role? defaultRole = await _roleRepository.GetByName(Role.DefaultUserRoleName, cancellationToken);

        if (defaultRole == null)
        {
            return Result.Failure<string>(RoleErrors.NotFound(Role.DefaultUserRoleName));
        }

        newUser.AssignRole(defaultRole);

        _userRepository.Add(newUser);
        await _userRepository.SaveChangesAsync();

        string token = _jwtTokenService.GenerateToken(newUser);

        return Result.Success(token);
    }
}
