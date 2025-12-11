using Core.Domain.Messaging;
using FluentAssertions;

namespace Core.UnitTests.Domain.Messaging;

public sealed class ErrorTests
{
    [Fact]
    public void Constructor_ShouldSetCodeAndDescription()
    {
        // Arrange
        const string code = "Test.Error";
        const string description = "Test error description";

        // Act
        var error = Error.Failure(code, description);

        // Assert
        error.Code.Should().Be(code);
        error.Description.Should().Be(description);
    }

    [Fact]
    public void None_ShouldHaveEmptyCodeAndDescription()
    {
        // Act
        var none = Error.None;

        // Assert
        none.Code.Should().Be(string.Empty);
        none.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void NullValue_ShouldHaveCorrectCodeAndDescription()
    {
        // Act
        var nullValue = Error.NullValue;

        // Assert
        nullValue.Code.Should().Be(Error.NullValue.Code);
        nullValue.Description.Should().Be(Error.NullValue.Description);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResult_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error");

        // Act
        Result result = error;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ToResult_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error");

        // Act
        var result = error.ToResult();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description");
        var error2 = Error.Failure("Test.Error", "Description");

        // Act & Assert
        error1.Should().Be(error2);
        (error1 == error2).Should().BeTrue();
        (error1 != error2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentCodes_ShouldReturnFalse()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error1", "Description");
        var error2 = Error.Failure("Test.Error2", "Description");

        // Act & Assert
        error1.Should().NotBe(error2);
        (error1 == error2).Should().BeFalse();
        (error1 != error2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentDescriptions_ShouldReturnFalse()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description1");
        var error2 = Error.Failure("Test.Error", "Description2");

        // Act & Assert
        error1.Should().NotBe(error2);
        (error1 == error2).Should().BeFalse();
        (error1 != error2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHash()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description");
        var error2 = Error.Failure("Test.Error", "Description");

        // Act & Assert
        error1.GetHashCode().Should().Be(error2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test description");

        // Act
        var result = error.ToString();

        // Assert
        result.Should().Contain("Test.Error");
        result.Should().Contain("Test description");
    }

    [Fact]
    public void Error_WithEmptyCode_ShouldBeValid()
    {
        // Act
        var error = Error.Failure(string.Empty, "Description");

        // Assert
        error.Code.Should().Be(string.Empty);
        error.Description.Should().Be("Description");
    }

    [Fact]
    public void Error_WithEmptyDescription_ShouldBeValid()
    {
        // Act
        var error = Error.Failure("Test.Error", string.Empty);

        // Assert
        error.Code.Should().Be("Test.Error");
        error.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Deconstruct_ShouldExtractCodeDescriptionAndType()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test description");

        // Act
        var (code, description, type) = error;

        // Assert
        code.Should().Be("Test.Error");
        description.Should().Be("Test description");
        type.Should().Be(ErrorType.Failure);
    }
}

