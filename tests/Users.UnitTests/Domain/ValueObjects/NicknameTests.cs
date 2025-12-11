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
        Assert.True(result.IsSuccess);
        Assert.Equal(validNickname, result.Value.Value);
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
        Assert.True(result.IsFailure);
        Assert.Equal(NicknameErrors.Empty.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongNickname_ShouldReturnFailure()
    {
        // Arrange
        string tooLongNickname = new string('a', Nickname.MaxLength + 1);

        // Act
        var result = Nickname.Create(tooLongNickname);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(NicknameErrors.TooLong.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithMaxLengthNickname_ShouldReturnSuccess()
    {
        // Arrange
        string maxLengthNickname = new string('a', Nickname.MaxLength);

        // Act
        var result = Nickname.Create(maxLengthNickname);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Equals_TwoNicknamesWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string nicknameValue = "CoolGamer";
        var nickname1 = Nickname.Create(nicknameValue).Value;
        var nickname2 = Nickname.Create(nicknameValue).Value;

        // Act & Assert
        Assert.Equal(nickname1, nickname2);
        Assert.True(nickname1.Equals(nickname2));
        Assert.Equal(nickname1.GetHashCode(), nickname2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoNicknamesWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var nickname1 = Nickname.Create("Gamer1").Value;
        var nickname2 = Nickname.Create("Gamer2").Value;

        // Act & Assert
        Assert.NotEqual(nickname1, nickname2);
        Assert.False(nickname1.Equals(nickname2));
    }
}
