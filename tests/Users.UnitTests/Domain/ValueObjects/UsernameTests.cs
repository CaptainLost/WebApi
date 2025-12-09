using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.ValueObjects;

public sealed class UsernameTests
{
    [Fact]
    public void Create_WithValidUsername_ShouldReturnSuccess()
    {
        // Arrange
        const string validUsername = "testuser";

        // Act
        var result = Username.Create(validUsername);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validUsername, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyUsername_ShouldReturnFailure(string? emptyUsername)
    {
        // Act
        var result = Username.Create(emptyUsername!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Username.Empty", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongUsername_ShouldReturnFailure()
    {
        // Arrange
        string tooLongUsername = new string('a', Username.MaxLength + 1);

        // Act
        var result = Username.Create(tooLongUsername);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Username.TooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithMaxLengthUsername_ShouldReturnSuccess()
    {
        // Arrange
        string maxLengthUsername = new string('a', Username.MaxLength);

        // Act
        var result = Username.Create(maxLengthUsername);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Equals_TwoUsernamesWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string usernameValue = "testuser";
        var username1 = Username.Create(usernameValue).Value;
        var username2 = Username.Create(usernameValue).Value;

        // Act & Assert
        Assert.Equal(username1, username2);
        Assert.True(username1.Equals(username2));
        Assert.Equal(username1.GetHashCode(), username2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoUsernamesWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var username1 = Username.Create("user1").Value;
        var username2 = Username.Create("user2").Value;

        // Act & Assert
        Assert.NotEqual(username1, username2);
        Assert.False(username1.Equals(username2));
    }
}
