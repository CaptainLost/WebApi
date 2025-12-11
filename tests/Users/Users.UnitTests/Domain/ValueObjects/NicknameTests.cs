using FluentAssertions;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.ValueObjects;

public sealed class NicknameTests
{
    [Fact]
    public void Create_WithValidNickname_ShouldReturnSuccess()
    {
        // Arrange
        const string validNickname = "CoolGamer123";

        // Act
        var result = Nickname.Create(validNickname);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validNickname);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyNickname_ShouldReturnFailure(string? emptyNickname)
    {
        // Act
        var result = Nickname.Create(emptyNickname!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NicknameErrors.Empty);
    }

    [Fact]
    public void Create_WithTooLongNickname_ShouldReturnFailure()
    {
        // Arrange
        string tooLongNickname = new string('a', Nickname.MaxLength + 1);

        // Act
        var result = Nickname.Create(tooLongNickname);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NicknameErrors.TooLong);
    }

    [Fact]
    public void Create_WithMaxLengthNickname_ShouldReturnSuccess()
    {
        // Arrange
        string maxLengthNickname = new string('a', Nickname.MaxLength);

        // Act
        var result = Nickname.Create(maxLengthNickname);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoNicknamesWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string nicknameValue = "CoolGamer";
        var nickname1 = Nickname.Create(nicknameValue).Value;
        var nickname2 = Nickname.Create(nicknameValue).Value;

        // Act & Assert
        nickname1.Should().Be(nickname2);
        nickname1.Equals(nickname2).Should().BeTrue();
        nickname1.GetHashCode().Should().Be(nickname2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoNicknamesWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var nickname1 = Nickname.Create("Gamer1").Value;
        var nickname2 = Nickname.Create("Gamer2").Value;

        // Act & Assert
        nickname1.Should().NotBe(nickname2);
        nickname1.Equals(nickname2).Should().BeFalse();
    }
}
