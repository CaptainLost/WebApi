using FluentAssertions;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.ValueObjects;

public sealed class PasswordTests
{
    [Fact]
    public void Create_WithValidHashAndSalt_ShouldReturnSuccess()
    {
        // Arrange
        string validHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] validSalt = new byte[PasswordHashingConstants.SaltSize];

        // Act
        var result = Password.Create(validHash, validSalt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Hash.Should().Be(validHash);
        result.Value.Salt.Should().Equal(validSalt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyHash_ShouldReturnFailure(string? emptyHash)
    {
        // Arrange
        byte[] validSalt = new byte[PasswordHashingConstants.SaltSize];

        // Act
        var result = Password.Create(emptyHash!, validSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.EmptyHash);
    }

    [Fact]
    public void Create_WithTooShortHash_ShouldReturnFailure()
    {
        // Arrange
        string shortHash = new string('A', PasswordHashingConstants.HashHexLength - 1);
        byte[] validSalt = new byte[PasswordHashingConstants.SaltSize];

        // Act
        var result = Password.Create(shortHash, validSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.HashTooShort);
    }

    [Fact]
    public void Create_WithEmptySalt_ShouldReturnFailure()
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
    public void Create_WithNullSalt_ShouldReturnFailure()
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
    public void Create_WithTooShortSalt_ShouldReturnFailure()
    {
        // Arrange
        string validHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] shortSalt = new byte[PasswordHashingConstants.SaltSize - 1];

        // Act
        var result = Password.Create(validHash, shortSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.SaltTooShort);
    }

    [Fact]
    public void Create_WithMinimumLengthHashAndSalt_ShouldReturnSuccess()
    {
        // Arrange
        string minHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] minSalt = new byte[PasswordHashingConstants.SaltSize];

        // Act
        var result = Password.Create(minHash, minSalt);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithLongerHashAndSalt_ShouldReturnSuccess()
    {
        // Arrange
        string longerHash = new string('A', PasswordHashingConstants.HashHexLength + 10);
        byte[] longerSalt = new byte[PasswordHashingConstants.SaltSize + 10];

        // Act
        var result = Password.Create(longerHash, longerSalt);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("short")]
    public void Create_WithVariousShortHashes_ShouldReturnFailure(string hash)
    {
        // Arrange
        byte[] salt = new byte[PasswordHashingConstants.SaltSize];

        // Act
        var result = Password.Create(hash, salt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.HashTooShort);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(32)]
    public void Create_WithVariousShortSalts_ShouldReturnFailure(int saltSize)
    {
        // Arrange
        string validHash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] shortSalt = new byte[saltSize];

        // Act
        var result = Password.Create(validHash, shortSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PasswordErrors.SaltTooShort);
    }

    [Fact]
    public void Equals_WithSameHashAndSalt_ShouldReturnTrue()
    {
        // Arrange
        string hash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] salt = new byte[PasswordHashingConstants.SaltSize];
        for (int i = 0; i < salt.Length; i++) salt[i] = (byte)i;

        var password1 = Password.Create(hash, salt).Value;
        var password2 = Password.Create(hash, salt).Value;

        // Act
        bool result = password1.Equals(password2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentHash_ShouldReturnFalse()
    {
        // Arrange
        string hash1 = new string('A', PasswordHashingConstants.HashHexLength);
        string hash2 = new string('B', PasswordHashingConstants.HashHexLength);
        byte[] salt = new byte[PasswordHashingConstants.SaltSize];

        var password1 = Password.Create(hash1, salt).Value;
        var password2 = Password.Create(hash2, salt).Value;

        // Act
        bool result = password1.Equals(password2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentSalt_ShouldReturnFalse()
    {
        // Arrange
        string hash = new string('A', PasswordHashingConstants.HashHexLength);
        byte[] salt1 = new byte[PasswordHashingConstants.SaltSize];
        byte[] salt2 = new byte[PasswordHashingConstants.SaltSize];
        salt2[0] = 1; // Make it different

        var password1 = Password.Create(hash, salt1).Value;
        var password2 = Password.Create(hash, salt2).Value;

        // Act
        bool result = password1.Equals(password2);

        // Assert
        result.Should().BeFalse();
    }
}
