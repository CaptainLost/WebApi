using Core.Domain.Messaging;
using FluentAssertions;
using Users.Application.Abstractions;
using Users.Application.Users.CreateUser;

namespace Users.UnitTests.Application;

public sealed class CreateUserCommandHandlerTests
{
    private readonly IAccountService _accountService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _accountService = A.Fake<IAccountService>();
        _handler = new CreateUserCommandHandler(_accountService);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldCallAuthenticationService()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");
        var expectedToken = "jwt-token-123";

        A.CallTo(() => _accountService.RegisterAsync(
            command.Username,
            command.Email,
            command.Password,
            A<CancellationToken>._))
            .Returns(Result.Success(expectedToken));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _accountService.RegisterAsync(
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
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");
        var expectedToken = "jwt-token-456";

        A.CallTo(() => _accountService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Success(expectedToken));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedToken);
    }

    [Fact]
    public async Task HandleAsync_WhenAuthenticationFails_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");
        var error = Error.Failure("Auth.Failed", "Registration failed");

        A.CallTo(() => _accountService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Failure<string>(error));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");
        var cancellationToken = new CancellationToken();

        A.CallTo(() => _accountService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            A<CancellationToken>._))
            .Returns(Result.Success("token"));

        // Act
        await _handler.HandleAsync(command, cancellationToken);

        // Assert
        A.CallTo(() => _accountService.RegisterAsync(
            A<string>._,
            A<string>._,
            A<string>._,
            cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
