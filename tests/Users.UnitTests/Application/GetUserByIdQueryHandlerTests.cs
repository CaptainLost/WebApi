using Core.Domain.Messaging;
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
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal("testuser", result.Value.Username);
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
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFoundById(userId), result.Error);
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
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal("specificUsername", result.Value.Username);
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
        Assert.True(result.IsFailure);
        Assert.Contains(userId.ToString(), result.Error.Description);
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
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.Equal("user1", result1.Value.Username);
        Assert.Equal("user2", result2.Value.Username);
    }

    private static User CreateValidUser(Guid id, string usernameValue)
    {
        var username = Username.Create(usernameValue).Value;
        var email = Email.Create($"{usernameValue}@example.com").Value;
        var nickname = Nickname.Create($"{usernameValue}Nick").Value;
        const string passwordHash = "hash";
        byte[] passwordSalt = [1, 2, 3, 4];

        return User.Create(id, username, email, passwordHash, passwordSalt, nickname).Value;
    }
}
