using FluentAssertions;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.ValueObjects;

public sealed class PlainPasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ShouldReturnSuccess()
    {
        // Arrange
        const string validPassword = "SecurePass123!";

        // Act
        var result = PlainPassword.Create(validPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validPassword);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyPassword_ShouldReturnFailure(string? emptyPassword)
    {
        // Act
        var result = PlainPassword.Create(emptyPassword!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.Empty.Code);
    }

    [Fact]
    public void Create_WithTooShortPassword_ShouldReturnFailure()
    {
        // Arrange
        string tooShortPassword = new string('a', PlainPassword.MinLength - 4) + "A1!";

        // Act
        var result = PlainPassword.Create(tooShortPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.TooShort.Code);
    }

    [Fact]
    public void Create_WithTooLongPassword_ShouldReturnFailure()
    {
        // Arrange
        string tooLongPassword = new string('a', PlainPassword.MaxLength + 1) + "A1!";

        // Act
        var result = PlainPassword.Create(tooLongPassword);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.TooLong.Code);
    }

    [Fact]
    public void Create_WithMinLengthPassword_ShouldReturnSuccess()
    {
        // Arrange
        string minLengthPassword = new string('a', PlainPassword.MinLength - 3) + "A1!";

        // Act
        var result = PlainPassword.Create(minLengthPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(minLengthPassword);
    }

    [Fact]
    public void Create_WithMaxLengthPassword_ShouldReturnSuccess()
    {
        // Arrange
        string maxLengthPassword = new string('a', PlainPassword.MaxLength - 3) + "A1!";

        // Act
        var result = PlainPassword.Create(maxLengthPassword);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("password123!")]
    [InlineData("passwordabc!")]
    public void Create_WithoutUppercase_ShouldReturnFailure(string password)
    {
        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.MissingUppercase.Code);
    }

    [Theory]
    [InlineData("PASSWORD123!")] // no lowercase
    [InlineData("PASSWORDABC!")]
    public void Create_WithoutLowercase_ShouldReturnFailure(string password)
    {
        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.MissingLowercase.Code);
    }

    [Theory]
    [InlineData("Password!")] // no digit
    [InlineData("Passwordabc!")]
    public void Create_WithoutDigit_ShouldReturnFailure(string password)
    {
        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.MissingDigit.Code);
    }

    [Theory]
    [InlineData("Password123")] // no special character
    [InlineData("Passwordabc123")]
    public void Create_WithoutSpecialCharacter_ShouldReturnFailure(string password)
    {
        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PlainPasswordErrors.MissingSpecialCharacter.Code);
    }

    [Theory]
    [InlineData("Pass@123")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Str0ng!Pass")]
    [InlineData("C0mpl3x#Pwd")]
    public void Create_WithVariousValidPasswords_ShouldReturnSuccess(string password)
    {
        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(password);
    }

    [Fact]
    public void Create_WithAllRequirements_ShouldReturnSuccess()
    {
        // Arrange
        string password = new string('a', PlainPassword.MinLength - 3) + "A1!";

        // Act
        var result = PlainPassword.Create(password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(password);
    }

    [Theory]
    [InlineData("Test@123", "Test@123", true)]
    [InlineData("Pass@123", "Pass@456", false)]
    public void Equals_ComparesTwoPasswords_ReturnsExpectedResult(string password1, string password2, bool expected)
    {
        // Arrange
        var plainPassword1 = PlainPassword.Create(password1).Value;
        var plainPassword2 = PlainPassword.Create(password2).Value;

        // Act
        bool result = plainPassword1.Equals(plainPassword2);

        // Assert
        result.Should().Be(expected);
    }
}
