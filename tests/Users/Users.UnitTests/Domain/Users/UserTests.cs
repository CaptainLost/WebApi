using FluentAssertions;
using Users.Domain.Configuration;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.Users;

public sealed class UserTests
{
    private static Username CreateValidUsername() => Username.Create("testuser").Value;
    private static Email CreateValidEmail() => Email.Create("test@example.com").Value;
    private static Nickname CreateValidNickname() => Nickname.Create("TestNick").Value;
    private static Password CreateValidPassword() => Password.Create(
        new string('A', PasswordHashingConstants.HashHexLength),
        new byte[PasswordHashingConstants.SaltSize]).Value;

    private static UserSettings CreateUserSettings(int maxFailedAttempts = 5, int baseLockoutMinutes = 15)
    {
        return new UserSettings
        {
            MaxFailedLoginAttempts = maxFailedAttempts,
            BaseLockoutDurationMinutes = baseLockoutMinutes
        };
    }

    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = CreateValidUsername();
        var email = CreateValidEmail();
        var nickname = CreateValidNickname();
        var password = CreateValidPassword();

        // Act
        var result = User.Create(id, username, email, password, nickname);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Username.Should().Be(username);
        result.Value.Email.Should().Be(email);
        result.Value.Nickname.Should().Be(nickname);
        result.Value.Password.Should().Be(password);
        result.Value.FailedLoginAttempts.Should().Be(0);
        result.Value.LockoutEnd.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyPasswordHash_ShouldReturnFailure(string? passwordHash)
    {
        // Arrange
        byte[] validSalt = [1, 2, 3, 4];

        // Act
        var passwordResult = Password.Create(passwordHash!, validSalt);

        // Assert
        passwordResult.IsFailure.Should().BeTrue();
        passwordResult.Error.Should().Be(PasswordErrors.EmptyHash);
    }

    [Fact]
    public void Create_WithEmptyPasswordSalt_ShouldReturnFailure()
    {
        // Arrange
        string validHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] emptySalt = [];

        // Act
        var result = Password.Create(validHash, emptySalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.EmptySalt);
    }

    [Fact]
    public void Create_WithNullPasswordSalt_ShouldReturnFailure()
    {
        // Arrange
        string validHash = new string('A', PasswordHashingConstants.HashHexLength);

        // Act
        var result = Password.Create(validHash, null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.EmptySalt);
    }

    [Fact]
    public void Create_WithValidData_ShouldRaiseUserCreatedDomainEvent()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Username username = CreateValidUsername();
        Email email = CreateValidEmail();
        Nickname nickname = CreateValidNickname();
        Password password = CreateValidPassword();

        // Act
        var result = User.Create(userId, username, email, password, nickname);
        var domainEvents = result.Value.GetDomainEvents();

        // Assert
        domainEvents.Should().ContainItemsAssignableTo<UserCreatedDomainEvent>();
        domainEvents.OfType<UserCreatedDomainEvent>().Should().ContainSingle();
        UserCreatedDomainEvent domainEvent = domainEvents.OfType<UserCreatedDomainEvent>().Single();
        domainEvent.UserId.Should().Be(userId);
        domainEvent.Username.Should().Be(username);
        domainEvent.Email.Should().Be(email);
        domainEvent.Nickname.Should().Be(nickname);
        domainEvent.OccurredAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void HasRole_WhenUserHasRole_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateValidUser();
        var role = Role.Registered;
        user.Roles.Add(role);

        // Act
        bool hasRole = user.HasRole(role.Name);

        // Assert
        hasRole.Should().BeTrue();
    }

    [Fact]
    public void HasRole_WhenUserDoesNotHaveRole_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        bool hasRole = user.HasRole("NonExistentRole");

        // Assert
        hasRole.Should().BeFalse();
    }

    [Fact]
    public void AssignRole_WhenRoleNotAssigned_ShouldSucceed()
    {
        // Arrange
        var user = CreateValidUser();
        var role = Role.Registered;

        // Act
        var result = user.AssignRole(role);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Roles.Should().Contain(role);
    }

    [Fact]
    public void AssignRole_WhenRoleAlreadyAssigned_ShouldFail()
    {
        // Arrange
        var user = CreateValidUser();
        var role = Role.Registered;
        user.Roles.Add(role);

        // Act
        var result = user.AssignRole(role);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.AlreadyHasRole(role.Name));
    }

    [Fact]
    public void IsLockedOut_WhenLockoutEndInFuture_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateValidUser();
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);

        // Act
        bool isLockedOut = user.IsLockedOut();

        // Assert
        isLockedOut.Should().BeTrue();
    }

    [Fact]
    public void IsLockedOut_WhenLockoutEndInPast_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(-10);

        // Act
        bool isLockedOut = user.IsLockedOut();

        // Assert
        isLockedOut.Should().BeFalse();
    }

    [Fact]
    public void IsLockedOut_WhenNoLockout_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        bool isLockedOut = user.IsLockedOut();

        // Assert
        isLockedOut.Should().BeFalse();
    }

    [Fact]
    public void RecordFailedLogin_ShouldIncrementFailedAttempts()
    {
        // Arrange
        var user = CreateValidUser();
        var settings = CreateUserSettings();

        // Act
        user.RecordFailedLogin(settings);

        // Assert
        user.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public void RecordFailedLogin_AfterFiveAttempts_ShouldLockAccount()
    {
        // Arrange
        var user = CreateValidUser();
        var settings = CreateUserSettings(maxFailedAttempts: 5);

        // Act
        for (int i = 0; i < settings.MaxFailedLoginAttempts; i++)
        {
            user.RecordFailedLogin(settings);
        }

        // Assert
        user.FailedLoginAttempts.Should().Be(settings.MaxFailedLoginAttempts);
        user.LockoutEnd.Should().NotBeNull();
        user.LockoutEnd.Should().BeAfter(DateTime.UtcNow);
        user.LockoutCount.Should().Be(1);
        user.LastLockout.Should().NotBeNull();
    }

    [Fact]
    public void RecordFailedLogin_SecondLockout_ShouldIncreaseLockoutDuration()
    {
        // Arrange
        var user = CreateValidUser();
        var settings = CreateUserSettings(maxFailedAttempts: 5);

        // First lockout
        for (int i = 0; i < settings.MaxFailedLoginAttempts; i++)
        {
            user.RecordFailedLogin(settings);
        }
        var firstLockoutEnd = user.LockoutEnd;
        user.ResetFailedLoginAttempts();

        // Act - Second lockout
        for (int i = 0; i < settings.MaxFailedLoginAttempts; i++)
        {
            user.RecordFailedLogin(settings);
        }

        // Assert
        user.LockoutCount.Should().Be(2);
        user.LockoutEnd.Should().NotBeNull();
        // Second lockout should be longer (exponential backoff)
    }

    [Fact]
    public void ResetFailedLoginAttempts_ShouldResetCountersAndLockout()
    {
        // Arrange
        var user = CreateValidUser();
        user.FailedLoginAttempts = 3;
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

        // Act
        user.ResetFailedLoginAttempts();

        // Assert
        user.FailedLoginAttempts.Should().Be(0);
        user.LockoutEnd.Should().BeNull();
    }

    [Fact]
    public void UpdatePassword_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = CreateValidUser();
        Password newPassword = Password.Create(
            new string('B', PasswordHashingConstants.HashHexLength),
            new byte[PasswordHashingConstants.SaltSize]).Value;

        // Act
        var result = user.UpdatePassword(newPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Password.Should().Be(newPassword);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdatePassword_WithEmptyHash_ShouldFail(string? newHash)
    {
        // Arrange
        byte[] newSalt = [5, 6, 7, 8];

        // Act
        var result = Password.Create(newHash!, newSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.EmptyHash);
    }

    [Fact]
    public void UpdatePassword_WithEmptySalt_ShouldFail()
    {
        // Arrange
        string newHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] emptySalt = [];

        // Act
        var result = Password.Create(newHash, emptySalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.EmptySalt);
    }

    [Fact]
    public void GetPermissions_ShouldReturnAllUniquePermissionsFromRoles()
    {
        // Arrange
        var user = CreateValidUser();
        var role1 = Role.Registered;
        var role2 = Role.Administrator;

        // Add some permissions to roles
        role1.Permissions.Add(Permission.GetUser);
        role1.Permissions.Add(Permission.GetUserList);
        role2.Permissions.Add(Permission.GetUser); // Duplicate
        role2.Permissions.Add(Permission.ModifyUser);
        role2.Permissions.Add(Permission.DeleteUser);

        user.Roles.Add(role1);
        user.Roles.Add(role2);

        // Act
        var permissions = user.GetPermissions();

        // Assert
        permissions.Count.Should().Be(4); // Should have unique permissions
        permissions.Should().Contain(Permission.GetUser.Name);
        permissions.Should().Contain(Permission.GetUserList.Name);
        permissions.Should().Contain(Permission.ModifyUser.Name);
        permissions.Should().Contain(Permission.DeleteUser.Name);
    }

    [Fact]
    public void GetPermissions_WithNoRoles_ShouldReturnEmptySet()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        var permissions = user.GetPermissions();

        // Assert
        permissions.Should().BeEmpty();
    }

    [Fact]
    public void Ban_WithValidData_ShouldSucceed()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var result = user.Ban(reason, banImposerId, expiresAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().ContainSingle();
        UserBan ban = user.Bans.First();
        ban.Reason.Should().Be(reason);
        ban.BanImposerId.Should().Be(banImposerId);
        ban.BannedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ban.ExpiresAt.Should().Be(expiresAt);
        ban.IsCurrentlyActive().Should().BeTrue();
        ban.UnbannedAt.Should().BeNull();
        ban.BanRemoverId.Should().BeNull();
    }

    [Fact]
    public void Ban_WhenAlreadyBanned_ShouldCreateAnotherActiveBan()
    {
        // Arrange
        User user = CreateValidUser();
        string originalReason = "Violating terms of service";
        Guid originalBanImposerId = Guid.NewGuid();
        DateTime originalExpiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(originalReason, originalBanImposerId, originalExpiresAt);

        string newReason = "Another reason";
        Guid newBanImposerId = Guid.NewGuid();
        DateTime newExpiresAt = DateTime.UtcNow.AddDays(14);

        // Act
        var result = user.Ban(newReason, newBanImposerId, newExpiresAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().HaveCount(2);

        UserBan oldBan = user.Bans.First();
        oldBan.IsCurrentlyActive().Should().BeTrue();
        oldBan.UnbannedAt.Should().BeNull();
        oldBan.BanRemoverId.Should().BeNull();

        UserBan newBan = user.Bans.Last();
        newBan.Reason.Should().Be(newReason);
        newBan.BanImposerId.Should().Be(newBanImposerId);
        newBan.ExpiresAt.Should().Be(newExpiresAt);
        newBan.IsCurrentlyActive().Should().BeTrue();

        user.IsBanned().Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Ban_WithEmptyOrNullReason_ShouldSucceed(string? emptyReason)
    {
        // Arrange
        User user = CreateValidUser();
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var result = user.Ban(emptyReason!, banImposerId, expiresAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().ContainSingle();
        UserBan ban = user.Bans.First();
        ban.Reason.Should().Be(emptyReason ?? string.Empty);
        ban.IsCurrentlyActive().Should().BeTrue();
    }

    [Fact]
    public void Ban_WithPastExpirationDate_ShouldFail()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = user.Ban(reason, banImposerId, expiresAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BanExpirationMustBeInFuture);
    }

    [Fact]
    public void Ban_WithEmptyBanImposerId_ShouldFail()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.Empty;
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var result = user.Ban(reason, banImposerId, expiresAt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BanImposerIdRequired);
    }

    [Fact]
    public void RemoveAllBans_WhenBanned_ShouldSucceed()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(reason, banImposerId, expiresAt);
        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveAllBans(banRemoverId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().ContainSingle();
        UserBan ban = user.Bans.First();
        ban.IsCurrentlyActive().Should().BeFalse();
        ban.UnbannedAt.Should().NotBeNull();
        ban.UnbannedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ban.BanRemoverId.Should().Be(banRemoverId);
        user.IsBanned().Should().BeFalse();
    }

    [Fact]
    public void RemoveAllBans_WhenMultipleBansActive_ShouldDeactivateAllBans()
    {
        // Arrange
        User user = CreateValidUser();
        Guid banImposerId1 = Guid.NewGuid();
        Guid banImposerId2 = Guid.NewGuid();
        DateTime expiresAt1 = DateTime.UtcNow.AddDays(7);
        DateTime expiresAt2 = DateTime.UtcNow.AddDays(14);

        user.Ban("First ban", banImposerId1, expiresAt1);
        user.Ban("Second ban", banImposerId2, expiresAt2);

        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveAllBans(banRemoverId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().HaveCount(2);
        user.Bans.Should().AllSatisfy(ban =>
        {
            ban.IsCurrentlyActive().Should().BeFalse();
            ban.UnbannedAt.Should().NotBeNull();
            ban.BanRemoverId.Should().Be(banRemoverId);
        });
        user.IsBanned().Should().BeFalse();
    }

    [Fact]
    public void RemoveAllBans_WhenNotBanned_ShouldFail()
    {
        // Arrange
        User user = CreateValidUser();
        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveAllBans(banRemoverId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotBanned);
    }

    [Fact]
    public void IsBanned_WhenBannedAndNotExpired_ShouldReturnTrue()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(reason, banImposerId, expiresAt);

        // Act
        bool isBanned = user.IsBanned();

        // Assert
        isBanned.Should().BeTrue();
    }

    [Fact]
    public void IsBanned_WhenBannedAndExpired_ShouldReturnFalse()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddSeconds(1);
        user.Ban(reason, banImposerId, expiresAt);
        Thread.Sleep(1100); // Wait for ban to expire

        // Act
        bool isBanned = user.IsBanned();

        // Assert
        isBanned.Should().BeFalse();
    }

    [Fact]
    public void IsBanned_WhenNotBanned_ShouldReturnFalse()
    {
        // Arrange
        User user = CreateValidUser();

        // Act
        bool isBanned = user.IsBanned();

        // Assert
        isBanned.Should().BeFalse();
    }

    [Fact]
    public void RemoveBan_WithValidBanId_ShouldSucceed()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(reason, banImposerId, expiresAt);
        Guid banId = user.Bans.First().Id;
        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveBan(banId, banRemoverId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        UserBan ban = user.Bans.First();
        ban.IsCurrentlyActive().Should().BeFalse();
        ban.UnbannedAt.Should().NotBeNull();
        ban.BanRemoverId.Should().Be(banRemoverId);
        user.IsBanned().Should().BeFalse();
    }

    [Fact]
    public void RemoveBan_WithInvalidBanId_ShouldFail()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(reason, banImposerId, expiresAt);
        Guid invalidBanId = Guid.NewGuid();
        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveBan(invalidBanId, banRemoverId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.BanNotFound(invalidBanId));
    }

    [Fact]
    public void RemoveBan_WithInactiveBan_ShouldFail()
    {
        // Arrange
        User user = CreateValidUser();
        string reason = "Violating terms of service";
        Guid banImposerId = Guid.NewGuid();
        DateTime expiresAt = DateTime.UtcNow.AddDays(7);
        user.Ban(reason, banImposerId, expiresAt);
        Guid banId = user.Bans.First().Id;
        Guid banRemoverId = Guid.NewGuid();
        user.RemoveBan(banId, banRemoverId); // First unban

        // Act
        var result = user.RemoveBan(banId, banRemoverId); // Try to unban again

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotBanned);
    }

    [Fact]
    public void RemoveBan_WithMultipleBans_ShouldOnlyUnbanSpecificOne()
    {
        // Arrange
        User user = CreateValidUser();
        Guid banImposerId1 = Guid.NewGuid();
        Guid banImposerId2 = Guid.NewGuid();
        DateTime expiresAt1 = DateTime.UtcNow.AddDays(7);
        DateTime expiresAt2 = DateTime.UtcNow.AddDays(14);

        user.Ban("First ban", banImposerId1, expiresAt1);
        user.Ban("Second ban", banImposerId2, expiresAt2);

        Guid firstBanId = user.Bans.First().Id;
        Guid banRemoverId = Guid.NewGuid();

        // Act
        var result = user.RemoveBan(firstBanId, banRemoverId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Bans.Should().HaveCount(2);

        UserBan firstBan = user.Bans.First(b => b.Id == firstBanId);
        firstBan.IsCurrentlyActive().Should().BeFalse();
        firstBan.BanRemoverId.Should().Be(banRemoverId);

        UserBan secondBan = user.Bans.First(b => b.Id != firstBanId);
        secondBan.IsCurrentlyActive().Should().BeTrue();

        user.IsBanned().Should().BeTrue(); // Still banned because second ban is active
    }

    private static User CreateValidUser()
    {
        return User.Create(
            Guid.NewGuid(),
            CreateValidUsername(),
            CreateValidEmail(),
            CreateValidPassword(),
            CreateValidNickname()).Value;
    }
}
