using Core.Domain.Messaging;
using FluentAssertions;

namespace Core.UnitTests.Domain.Messaging;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessGeneric_ShouldCreateSuccessResultWithValue()
    {
        // Arrange
        const string value = "test value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void FailureGeneric_ShouldCreateFailureResultWithoutValue()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void FailureResult_AccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "Test error description");
        var result = Result.Failure<string>(error);

        // Act & Assert
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Create_WithNonNullValue_ShouldCreateSuccessResult()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithNullValue_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Result.Create(value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void Success_WithError_ShouldNotBeValid()
    {
        // Arrange
        var successResult = Result.Success();
        var error = Error.Failure("Test.Error", "Test error");

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.Error.Should().NotBe(error);
        successResult.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_WithNoError_ShouldRequireError()
    {
        // Arrange & Act
        var failureResult = Result.Failure(Error.Failure("Test", "Test"));

        // Assert
        failureResult.IsFailure.Should().BeTrue();
        failureResult.Error.Should().NotBe(Error.None);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        // Act
        Result<int> result = 42;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_FromNullValue_ShouldCreateFailureResult()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void IsSuccess_ShouldBeOppositeOfIsFailure()
    {
        // Arrange
        var successResult = Result.Success();
        var failureResult = Result.Failure(Error.Failure("Test", "Test"));

        // Assert
        successResult.IsSuccess.Should().BeTrue();
        successResult.IsFailure.Should().BeFalse();
        failureResult.IsSuccess.Should().BeFalse();
        failureResult.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void SuccessGeneric_WithComplexObject_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        var complexObject = new { Name = "Test", Value = 42 };

        // Act
        var result = Result.Success(complexObject);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(complexObject);
        result.Value.Name.Should().Be("Test");
        result.Value.Value.Should().Be(42);
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
        stringResult.Should().BeOfType<Result<string>>();
        intResult.Should().BeOfType<Result<int>>();
        stringResult.Error.Should().Be(error);
        intResult.Error.Should().Be(error);
    }
}

