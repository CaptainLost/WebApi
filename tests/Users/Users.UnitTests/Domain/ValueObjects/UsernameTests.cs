using FluentAssertions;
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
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validUsername);
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
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UsernameErrors.Empty);
    }

    [Fact]
    public void Create_WithTooLongUsername_ShouldReturnFailure()
    {
        // Arrange
        string tooLongUsername = new string('a', Username.MaxLength + 1);

        // Act
        var result = Username.Create(tooLongUsername);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UsernameErrors.TooLong);
    }

    [Fact]
    public void Create_WithMaxLengthUsername_ShouldReturnSuccess()
    {
        // Arrange
        string maxLengthUsername = new string('a', Username.MaxLength);

        // Act
        var result = Username.Create(maxLengthUsername);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoUsernamesWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string usernameValue = "testuser";
        var username1 = Username.Create(usernameValue).Value;
        var username2 = Username.Create(usernameValue).Value;

        // Act & Assert
        username1.Should().Be(username2);
        username1.Equals(username2).Should().BeTrue();
        username1.GetHashCode().Should().Be(username2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoUsernamesWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var username1 = Username.Create("user1").Value;
        var username2 = Username.Create("user2").Value;

        // Act & Assert
        username1.Should().NotBe(username2);
        username1.Equals(username2).Should().BeFalse();
    }
}
