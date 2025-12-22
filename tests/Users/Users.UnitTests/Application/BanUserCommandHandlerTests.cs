using Core.Domain.Messaging;
using FakeItEasy;
using FluentAssertions;
using Users.Application.Users.BanUser;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Application;

public sealed class BanUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly BanUserCommandHandler _handler;

    public BanUserCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _handler = new BanUserCommandHandler(_userRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldBanUserAndReturnSuccess()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banImposerId = Guid.NewGuid();
        string reason = "Violating terms of service";
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        User user = CreateValidUser(userId);

        var command = new BanUserCommand(userId, reason, banImposerId, expiresAt);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsBanned().Should().BeTrue();
        user.Bans.Should().ContainSingle();
        UserBan ban = user.Bans.First();
        ban.Reason.Should().Be(reason);
        ban.BanImposerId.Should().Be(banImposerId);
        ban.ExpiresAt.Should().Be(expiresAt);
        ban.IsCurrentlyActive().Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banImposerId = Guid.NewGuid();
        string reason = "Violating terms of service";
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        var command = new BanUserCommand(userId, reason, banImposerId, expiresAt);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns((User?)null);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFoundById(userId));
    }

    [Fact]
    public async Task HandleAsync_WhenUserAlreadyBanned_ShouldAddAnotherBan()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid newBanImposerId = Guid.NewGuid();
        string newReason = "Violating terms of service";
        DateTime newExpiresAt = DateTime.UtcNow.AddDays(7);

        User user = CreateValidUser(userId);
        user.Ban("Previous reason", Guid.NewGuid(), DateTime.UtcNow.AddDays(1));

        var command = new BanUserCommand(userId, newReason, newBanImposerId, newExpiresAt);

        A.CallTo(() => _userRepository.GetByIdWithBansAsync(userId, A<CancellationToken>._))
            .Returns(user);

        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().HaveCount(2);
        var activeBans = user.Bans.Where(b => b.IsCurrentlyActive()).ToList();
        activeBans.Should().HaveCount(2);
        UserBan newBan = user.Bans.Last();
        newBan.Reason.Should().Be(newReason);
        newBan.BanImposerId.Should().Be(newBanImposerId);
        newBan.ExpiresAt.Should().Be(newExpiresAt);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChangesAsync()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid banImposerId = Guid.NewGuid();
        string reason = "Violating terms of service";
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        User user = CreateValidUser(userId);

        var command = new BanUserCommand(userId, reason, banImposerId, expiresAt);

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
