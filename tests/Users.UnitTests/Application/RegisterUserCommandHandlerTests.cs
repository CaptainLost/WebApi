using Core.Domain.Messaging;
using Users.Application.Abstractions;
using Users.Application.Users.RegisterUser;

namespace Users.UnitTests.Application;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _authenticationService = A.Fake<IAuthenticationService>();
        _handler = new RegisterUserCommandHandler(_authenticationService);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldCallAuthenticationService()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "Password123!");
        var expectedToken = "jwt-token-123";
        
        A.CallTo(() => _authenticationService.RegisterAsync(
            command.Username,
            command.Email,
            command.Password,
            A<CancellationToken>._))
            .Returns(Result.Success(expectedToken));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _authenticationService.RegisterAsync(
            command.Username,
            command.Email,
            command.Password,
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTokenFromAuthenticationService()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "Password123!");
        var expectedToken = "jwt-token-456";
        
        A.CallTo(() => _authenticationService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Success(expectedToken));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedToken, result.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenAuthenticationFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "Password123!");
        var error = Error.Failure("Auth.Failed", "Registration failed");
        
        A.CallTo(() => _authenticationService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Failure<string>(error));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "test@example.com", "Password123!");
        var cancellationToken = new CancellationToken();
        
        A.CallTo(() => _authenticationService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Success("token"));

        // Act
        await _handler.HandleAsync(command, cancellationToken);

        // Assert
        A.CallTo(() => _authenticationService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
