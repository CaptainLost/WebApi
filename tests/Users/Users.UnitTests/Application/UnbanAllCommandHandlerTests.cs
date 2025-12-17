using Core.Domain.Messaging;
using FakeItEasy;
using FluentAssertions;
using Users.Application.Users.UnbanAll;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class UnbanAllCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly UnbanAllCommandHandler _handler;

    public UnbanAllCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new UnbanAllCommandHandler(_userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldUnbanAllBansAndReturnSuccess()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid unbannedBy = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("Violating terms", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        var command = new UnbanAllCommand(userId, unbannedBy);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsBanned().Should().BeFalse();
        user.Bans.Should().ContainSingle();
        UserBan ban = user.Bans.First();
        ban.IsCurrentlyActive().Should().BeFalse();
        ban.UnbannedAt.Should().NotBeNull();
        ban.UnbannedBy.Should().Be(unbannedBy);
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid unbannedBy = Guid.NewGuid();

        var command = new UnbanAllCommand(userId, unbannedBy);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFoundById(userId));
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotBanned_ShouldReturnFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid unbannedBy = Guid.NewGuid();

        User user = CreateValidUser(userId);

        var command = new UnbanAllCommand(userId, unbannedBy);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotBanned);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChangesAsync()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid unbannedBy = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("Violating terms", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        var command = new UnbanAllCommand(userId, unbannedBy);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _userRepository.SaveChangesAsync())
            .MustHaveHappenedOnceExactly();
    }

    private static User CreateValidUser(Guid userId)
    {
        return User.Create(
            userId,
            Username.Create("testuser").Value,
            Email.Create("test@example.com").Value,
            Password.Create(
                new string('A', PasswordHashingConstants.HashHexLength),
                new byte[PasswordHashingConstants.SaltSize]).Value,
            Nickname.Create("TestNick").Value).Value;
    }
}
