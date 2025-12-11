using Core.Domain.Messaging;

namespace Core.UnitTests.Domain.Messaging;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void SuccessGeneric_ShouldCreateSuccessResultWithValue()
    {
        // Arrange
        const string value = "test value";

        // Act
        var result = Result.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void FailureGeneric_ShouldCreateFailureResultWithoutValue()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void FailureResult_AccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");
        var result = Result.Failure<string>(error);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Create_WithNonNullValue_ShouldCreateSuccessResult()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result.Create(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Create_WithNullValue_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Result.Create(value);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void Success_WithError_ShouldNotBeValid()
    {
        // Arrange
        var successResult = Result.Success();
        var error = Error.Failure("Test.Error", "Test error");

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.NotEqual(error, successResult.Error);
        Assert.Equal(Error.None, successResult.Error);
    }

    [Fact]
    public void Failure_WithNoError_ShouldRequireError()
    {
        // Arrange & Act
        var failureResult = Result.Failure(Error.Failure("Test", "Test"));

        // Assert
        Assert.True(failureResult.IsFailure);
        Assert.NotEqual(Error.None, failureResult.Error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        // Act
        Result<int> result = 42;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromNullValue_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void IsSuccess_ShouldBeOppositeOfIsFailure()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(Error.Failure("Test", "Test"));

        // Assert
        Assert.True(successResult.IsSuccess);
        Assert.False(successResult.IsFailure);
        Assert.False(failureResult.IsSuccess);
        Assert.True(failureResult.IsFailure);
    }

    [Fact]
    public void SuccessGeneric_WithComplexObject_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        var complexObject = new { Name = "Test", Value = 42 };

        // Act
        var result = Result.Success(complexObject);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(complexObject, result.Value);
        Assert.Equal("Test", result.Value.Name);
        Assert.Equal(42, result.Value.Value);
    }

    [Fact]
    public void FailureGeneric_WithDifferentTypes_ShouldMaintainTypeInformation()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error");

        // Act
        var stringResult = Result.Failure<string>(error);
        var intResult = Result.Failure<int>(error);

        // Assert
        Assert.IsType<Result<string>>(stringResult);
        Assert.IsType<Result<int>>(intResult);
        Assert.Equal(error, stringResult.Error);
        Assert.Equal(error, intResult.Error);
    }
}

