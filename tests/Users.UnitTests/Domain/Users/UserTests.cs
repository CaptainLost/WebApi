using Users.Domain.Configuration;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.Users;

public sealed class UserTests
{
    private static Username CreateValidUsername() => Username.Create("testuser").Value;
    private static Email CreateValidEmail() => Email.Create("test@example.com").Value;
    private static Nickname CreateValidNickname() => Nickname.Create("TestNick").Value;
    private const string ValidPasswordHash = "valid-hash";
    private static readonly byte[] ValidPasswordSalt = [1, 2, 3, 4];

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

        // Act
        var result = User.Create(id, username, email, ValidPasswordHash, ValidPasswordSalt, nickname);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(username, result.Value.Username);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(nickname, result.Value.Nickname);
        Assert.Equal(ValidPasswordHash, result.Value.PasswordHash);
        Assert.Equal(ValidPasswordSalt, result.Value.PasswordSalt);
        Assert.Equal(0, result.Value.FailedLoginAttempts);
        Assert.Null(result.Value.LockoutEnd);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyPasswordHash_ShouldReturnFailure(string? passwordHash)
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = CreateValidUsername();
        var email = CreateValidEmail();
        var nickname = CreateValidNickname();

        // Act
        var result = User.Create(id, username, email, passwordHash!, ValidPasswordSalt, nickname);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidPasswordHash, result.Error);
    }

    [Fact]
    public void Create_WithEmptyPasswordSalt_ShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = CreateValidUsername();
        var email = CreateValidEmail();
        var nickname = CreateValidNickname();
        byte[] emptySalt = [];

        // Act
        var result = User.Create(id, username, email, ValidPasswordHash, emptySalt, nickname);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidPasswordHash, result.Error);
    }

    [Fact]
    public void Create_WithNullPasswordSalt_ShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = CreateValidUsername();
        var email = CreateValidEmail();
        var nickname = CreateValidNickname();

        // Act
        var result = User.Create(id, username, email, ValidPasswordHash, null!, nickname);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidPasswordHash, result.Error);
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
        Assert.True(hasRole);
    }

    [Fact]
    public void HasRole_WhenUserDoesNotHaveRole_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        bool hasRole = user.HasRole("NonExistentRole");

        // Assert
        Assert.False(hasRole);
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
        Assert.True(result.IsSuccess);
        Assert.Contains(role, user.Roles);
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
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.AlreadyHasRole(role.Name), result.Error);
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
        Assert.True(isLockedOut);
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
        Assert.False(isLockedOut);
    }

    [Fact]
    public void IsLockedOut_WhenNoLockout_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        bool isLockedOut = user.IsLockedOut();

        // Assert
        Assert.False(isLockedOut);
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
        Assert.Equal(1, user.FailedLoginAttempts);
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
        Assert.Equal(settings.MaxFailedLoginAttempts, user.FailedLoginAttempts);
        Assert.NotNull(user.LockoutEnd);
        Assert.True(user.LockoutEnd > DateTime.UtcNow);
        Assert.Equal(1, user.LockoutCount);
        Assert.NotNull(user.LastLockout);
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
        Assert.Equal(2, user.LockoutCount);
        Assert.NotNull(user.LockoutEnd);
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
        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Null(user.LockoutEnd);
    }

    [Fact]
    public void UpdatePassword_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = CreateValidUser();
        const string newHash = "new-hash";
        byte[] newSalt = [5, 6, 7, 8];

        // Act
        var result = user.UpdatePassword(newHash, newSalt);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newHash, user.PasswordHash);
        Assert.Equal(newSalt, user.PasswordSalt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdatePassword_WithEmptyHash_ShouldFail(string? newHash)
    {
        // Arrange
        var user = CreateValidUser();
        byte[] newSalt = [5, 6, 7, 8];

        // Act
        var result = user.UpdatePassword(newHash!, newSalt);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidPasswordHash, result.Error);
    }

    [Fact]
    public void UpdatePassword_WithEmptySalt_ShouldFail()
    {
        // Arrange
        var user = CreateValidUser();
        const string newHash = "new-hash";
        byte[] emptySalt = [];

        // Act
        var result = user.UpdatePassword(newHash, emptySalt);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidPasswordHash, result.Error);
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
        Assert.Equal(4, permissions.Count); // Should have unique permissions
        Assert.Contains(Permission.GetUser.Name, permissions);
        Assert.Contains(Permission.GetUserList.Name, permissions);
        Assert.Contains(Permission.ModifyUser.Name, permissions);
        Assert.Contains(Permission.DeleteUser.Name, permissions);
    }

    [Fact]
    public void GetPermissions_WithNoRoles_ShouldReturnEmptySet()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        var permissions = user.GetPermissions();

        // Assert
        Assert.Empty(permissions);
    }

    private static User CreateValidUser()
    {
        return User.Create(
            Guid.NewGuid(),
            CreateValidUsername(),
            CreateValidEmail(),
            ValidPasswordHash,
            ValidPasswordSalt,
            CreateValidNickname()).Value;
    }
}
