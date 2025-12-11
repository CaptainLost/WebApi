using Core.Domain.Messaging;
using FluentAssertions;
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
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validEmail);
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
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOneOf(EmailErrors.Empty, Error.NullValue);
    }

    [Fact]
    public void Create_WithTooLongEmail_ShouldReturnFailure()
    {
        // Arrange
        string tooLongEmail = new string('a', Email.MaxLength - 10) + "@example.com";

        // Act
        var result = Email.Create(tooLongEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(EmailErrors.TooLong);
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
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(EmailErrors.InvalidFormat);
    }

    [Fact]
    public void Create_WithInvalidFormat_MultipleAtSigns_ShouldReturnFailure()
    {
        // Arrange
        const string invalidEmail = "test@@example.com";

        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(EmailErrors.InvalidFormat);
    }

    [Fact]
    public void Equals_TwoEmailsWithSameValue_ShouldBeEqual()
    {
        // Arrange
        const string emailValue = "test@example.com";
        var email1 = Email.Create(emailValue).Value;
        var email2 = Email.Create(emailValue).Value;

        // Act & Assert
        email1.Should().Be(email2);
        email1.Equals(email2).Should().BeTrue();
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void Equals_TwoEmailsWithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        // Act & Assert
        email1.Should().NotBe(email2);
        email1.Equals(email2).Should().BeFalse();
    }
}
