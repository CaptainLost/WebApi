using Core.Domain.Messaging;
using FluentAssertions;
using Users.Application.Abstractions;
using Users.Application.Users.LoginUser;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class LoginUserCommandHandlerTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _authenticationService = A.Fake<IAuthenticationService>();
        _userRepository = A.Fake<IUserRepository>();
        _handler = new LoginUserCommandHandler(_authenticationService, _userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "Password123!");
        var user = CreateValidUser();
        var expectedToken = "jwt-token-123";
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns(user);
        
        A.CallTo(() => _authenticationService.LoginAsync(user, command.Password, A<CancellationToken>._))
            .Returns(Result.Success(expectedToken));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedToken);
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnInvalidCredentials()
    {
        // Arrange
        var command = new LoginUserCommand("nonexistentuser", "Password123!");
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
        
        A.CallTo(() => _authenticationService.LoginAsync(A<User>._, A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task HandleAsync_WhenAuthenticationFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "WrongPassword");
        var user = CreateValidUser();
        var error = Error.Failure("Auth.InvalidPassword", "Invalid password");
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns(user);
        
        A.CallTo(() => _authenticationService.LoginAsync(user, command.Password, A<CancellationToken>._))
            .Returns(Result.Failure<string>(error));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateUsernameFromCommand()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "Password123!");
        var user = CreateValidUser();
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns(user);
        
        A.CallTo(() => _authenticationService.LoginAsync(A<User>._, A<string>._, A<CancellationToken>._))
            .Returns(Result.Success("token"));

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetByUsernameAsync(
            A<Username>.That.Matches(u => u.Value == command.Username),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldCallAuthenticationServiceWithCorrectUser()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "Password123!");
        var user = CreateValidUser();
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns(user);
        
        A.CallTo(() => _authenticationService.LoginAsync(A<User>._, A<string>._, A<CancellationToken>._))
            .Returns(Result.Success("token"));

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _authenticationService.LoginAsync(
            user,
            command.Password,
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new LoginUserCommand("testuser", "Password123!");
        var user = CreateValidUser();
        var cancellationToken = new CancellationToken();
        
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, A<CancellationToken>._))
            .Returns(user);
        
        A.CallTo(() => _authenticationService.LoginAsync(A<User>._, A<string>._, A<CancellationToken>._))
            .Returns(Result.Success("token"));

        // Act
        await _handler.HandleAsync(command, cancellationToken);

        // Assert
        A.CallTo(() => _userRepository.GetByUsernameAsync(A<Username>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => _authenticationService.LoginAsync(A<User>._, A<string>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static User CreateValidUser()
    {
        var username = Username.Create("testuser").Value;
        var email = Email.Create("test@example.com").Value;
        var nickname = Nickname.Create("TestNick").Value;
        var password = Password.Create(
            new string('A', PasswordHashingConstants.HashHexLength),
            new byte[PasswordHashingConstants.SaltSize]).Value;

        return User.Create(Guid.NewGuid(), username, email, password, nickname).Value;
    }
}
