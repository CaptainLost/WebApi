using Core.Domain.Messaging;
using FakeItEasy;
using FluentAssertions;
using Users.Application.Users.UnbanSingleUser;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class UnbanSingleUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly RemoveSingleUserBanCommandHandler _handler;

    public UnbanSingleUserCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new RemoveSingleUserBanCommandHandler(_userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidBanId_ShouldUnbanSingleAndReturnSuccess()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("Test reason", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        Guid banId = user.Bans.First().Id;

        var command = new RemoveSingleUserBanCommand(userId, banId, banRemoverId);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsBanned().Should().BeFalse();
        UserBan ban = user.Bans.First();
        ban.IsCurrentlyActive().Should().BeFalse();
        ban.BanRemoverId.Should().Be(banRemoverId);
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        var command = new RemoveSingleUserBanCommand(userId, banId, banRemoverId);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFoundById(userId));
    }

    [Fact]
    public async Task HandleAsync_WithInvalidBanId_ShouldReturnFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid invalidBanId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("Test reason", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        var command = new RemoveSingleUserBanCommand(userId, invalidBanId, banRemoverId);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BanNotFound(invalidBanId));
    }

    [Fact]
    public async Task HandleAsync_WithMultipleBans_ShouldOnlyUnbanSpecificOne()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("First ban", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        user.Ban("Second ban", Guid.NewGuid(), DateTime.UtcNow.AddDays(14));

        Guid firstBanId = user.Bans.First().Id;

        var command = new RemoveSingleUserBanCommand(userId, firstBanId, banRemoverId);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().HaveCount(2);

        UserBan unbannedBan = user.Bans.First(b => b.Id == firstBanId);
        unbannedBan.IsCurrentlyActive().Should().BeFalse();

        UserBan stillActiveBan = user.Bans.First(b => b.Id != firstBanId);
        stillActiveBan.IsCurrentlyActive().Should().BeTrue();

        user.IsBanned().Should().BeTrue(); // Still banned due to second ban
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChangesAsync()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        User user = CreateValidUser(userId);
        user.Ban("Test reason", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        Guid banId = user.Bans.First().Id;

        var command = new RemoveSingleUserBanCommand(userId, banId, banRemoverId);

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
