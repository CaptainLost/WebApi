using Core.Domain.Messaging;
using Users.Application.Abstractions;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<string>> LoginAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        if (user.IsLockedOut())
        {
            return Result.Failure<string>(UserErrors.AccountLockedOut);
        }

        bool isPasswordValid = _passwordHashingService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
        {
            user.RecordFailedLogin();
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

        string passwordHash = _passwordHashingService.HashPassword(password, out byte[] passwordSalt);

        Result<User> createUserResult = User.Create(Guid.NewGuid(), usernameResult.Value, emailResult.Value, passwordHash, passwordSalt, nicknameResult.Value);
        if (createUserResult.IsFailure)
        {
            return Result.Failure<string>(createUserResult.Error);
        }

        User newUser = createUserResult.Value;

        newUser.AssignRole(Role.DefaultUserRole);

        _userRepository.Add(newUser);
        await _userRepository.SaveChangesAsync();

        string token = _jwtTokenService.GenerateToken(newUser);

        return Result.Success(token);
    }
}
