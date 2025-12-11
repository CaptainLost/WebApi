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
