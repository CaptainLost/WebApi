using Core.Domain.Messaging;
using Users.Domain.ValueObjects;

namespace Users.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        const string validEmail = "test@example.com";

        // Act
        var result = Email.Create(validEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validEmail, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ShouldReturnFailure(string? emptyEmail)
    {
        // Act
        var result = Email.Create(emptyEmail!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.True(result.Error.Code == EmailErrors.Empty.Code || result.Error.Code == Error.NullValue.Code);
    }

    [Fact]
    public void Create_WithTooLongEmail_ShouldReturnFailure()
    {
        // Arrange
        string tooLongEmail = new string('a', Email.MaxLength - 10) + "@example.com";

        // Act
        var result = Email.Create(tooLongEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(EmailErrors.TooLong.Code, result.Error.Code);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid.email.com")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    public void Create_WithInvalidFormat_NoAtSign_ShouldReturnFailure(string invalidEmail)
    {
        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(EmailErrors.InvalidFormat.Code, result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidFormat_MultipleAtSigns_ShouldReturnFailure()
    {
        // Arrange
        const string invalidEmail = "test@@example.com";

        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(EmailErrors.InvalidFormat.Code, result.Error.Code);
    }

    [Fact]
    public void Equals_TwoEmailsWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string emailValue = "test@example.com";
        var email1 = Email.Create(emailValue).Value;
        var email2 = Email.Create(emailValue).Value;

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1.Equals(email2));
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoEmailsWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1.Equals(email2));
    }
}
