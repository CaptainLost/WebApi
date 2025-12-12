using FluentAssertions;
using Users.Application.Users.GetUserById;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = CreateValidUser(userId, "testuser");

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(userId);
        result.Value.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUser_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFoundById(userId));
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = CreateValidUser(userId, "testuser");

        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(user);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = CreateValidUser(userId, "testuser");
        var cancellationToken = new CancellationToken();

        A.CallTo(() => _userRepository.GetByIdAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(user);

        // Act
        await _handler.HandleAsync(query, cancellationToken);

        // Assert
        A.CallTo(() => _userRepository.GetByIdAsync(userId, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleAsync_WithExistingUser_ShouldMapToResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = CreateValidUser(userId, "specificUsername");

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(userId);
        result.Value.Username.Should().Be("specificUsername");
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldIncludeUserIdInError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        A.CallTo(() => _userRepository.GetByIdAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Description.Should().Contain(userId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithDifferentUserIds_ShouldReturnDifferentUsers()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var user1 = CreateValidUser(userId1, "user1");
        var user2 = CreateValidUser(userId2, "user2");

        A.CallTo(() => _userRepository.GetByIdAsync(userId1, A<CancellationToken>._))
            .Returns(user1);
        A.CallTo(() => _userRepository.GetByIdAsync(userId2, A<CancellationToken>._))
            .Returns(user2);

        // Act
        var query1 = new GetUserByIdQuery(userId1);
        var query2 = new GetUserByIdQuery(userId2);
        var result1 = await _handler.HandleAsync(query1, CancellationToken.None);
        var result2 = await _handler.HandleAsync(query2, CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result1.Value.Username.Should().Be("user1");
        result2.Value.Username.Should().Be("user2");
    }

    private static User CreateValidUser(Guid id, string usernameValue)
    {
        var username = Username.Create(usernameValue).Value;
        var email = Email.Create($"{usernameValue}@example.com").Value;
        var nickname = Nickname.Create($"{usernameValue}Nick").Value;
        var password = Password.Create(
            new string('A', PasswordHashingConstants.HashHexLength),
            new byte[PasswordHashingConstants.SaltSize]).Value;

        return User.Create(id, username, email, password, nickname).Value;
    }
}
