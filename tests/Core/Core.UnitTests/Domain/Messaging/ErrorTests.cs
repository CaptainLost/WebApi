using Core.Domain.Messaging;

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
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void None_ShouldHaveEmptyCodeAndDescription()
    {
        // Act
        var none = Error.None;

        // Assert
        Assert.Equal(string.Empty, none.Code);
        Assert.Equal(string.Empty, none.Description);
    }

    [Fact]
    public void NullValue_ShouldHaveCorrectCodeAndDescription()
    {
        // Act
        var nullValue = Error.NullValue;

        // Assert
        Assert.Equal(Error.NullValue.Code, nullValue.Code);
        Assert.Equal(Error.NullValue.Description, nullValue.Description);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResult_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error");

        // Act
        Result result = error;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ToResult_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error");

        // Act
        var result = error.ToResult();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description");
        var error2 = Error.Failure("Test.Error", "Description");

        // Act & Assert
        Assert.Equal(error1, error2);
        Assert.True(error1 == error2);
        Assert.False(error1 != error2);
    }

    [Fact]
    public void Equals_WithDifferentCodes_ShouldReturnFalse()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error1", "Description");
        var error2 = Error.Failure("Test.Error2", "Description");

        // Act & Assert
        Assert.NotEqual(error1, error2);
        Assert.False(error1 == error2);
        Assert.True(error1 != error2);
    }

    [Fact]
    public void Equals_WithDifferentDescriptions_ShouldReturnFalse()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description1");
        var error2 = Error.Failure("Test.Error", "Description2");

        // Act & Assert
        Assert.NotEqual(error1, error2);
        Assert.False(error1 == error2);
        Assert.True(error1 != error2);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHash()
    {
        // Arrange
        var error1 = Error.Failure("Test.Error", "Description");
        var error2 = Error.Failure("Test.Error", "Description");

        // Act & Assert
        Assert.Equal(error1.GetHashCode(), error2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test description");

        // Act
        var result = error.ToString();

        // Assert
        Assert.Contains("Test.Error", result);
        Assert.Contains("Test description", result);
    }

    [Fact]
    public void Error_WithEmptyCode_ShouldBeValid()
    {
        // Act
        var error = Error.Failure(string.Empty, "Description");

        // Assert
        Assert.Equal(string.Empty, error.Code);
        Assert.Equal("Description", error.Description);
    }

    [Fact]
    public void Error_WithEmptyDescription_ShouldBeValid()
    {
        // Act
        var error = Error.Failure("Test.Error", string.Empty);

        // Assert
        Assert.Equal("Test.Error", error.Code);
        Assert.Equal(string.Empty, error.Description);
    }

    [Fact]
    public void Deconstruct_ShouldExtractCodeDescriptionAndType()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test description");

        // Act
        var (code, description, type) = error;

        // Assert
        Assert.Equal("Test.Error", code);
        Assert.Equal("Test description", description);
        Assert.Equal(ErrorType.Failure, type);
    }
}

