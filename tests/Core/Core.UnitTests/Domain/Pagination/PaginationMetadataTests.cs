using Core.Domain.Messaging;
using Core.Domain.Pagination;
using FluentAssertions;

namespace Core.UnitTests.Domain.Pagination;

public sealed class PaginationMetadataTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePaginationMetadata()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 2, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(100);
        result.Value.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalPages.Should().Be(10);
    }

    [Fact]
    public void Create_WithNegativeTotalCount_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: -1, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaginationErrors.NegativeTotalCount);
    }

    [Fact]
    public void Create_WithZeroPageNumber_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 0, pageSize: 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaginationErrors.InvalidPageNumber);
    }

    [Fact]
    public void Create_WithNegativePageNumber_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: -1, pageSize: 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaginationErrors.InvalidPageNumber);
    }

    [Fact]
    public void Create_WithZeroPageSize_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaginationErrors.InvalidPageSize);
    }

    [Fact]
    public void Create_WithNegativePageSize_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: -5);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PaginationErrors.InvalidPageSize);
    }

    [Fact]
    public void TotalPages_WithEvenDivision_ShouldCalculateCorrectly()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(10);
    }

    [Fact]
    public void TotalPages_WithUnevenDivision_ShouldRoundUp()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 105, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(11);
    }

    [Fact]
    public void TotalPages_WithZeroTotalCount_ShouldReturnZero()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 0, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(0);
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 2, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasPreviousPage_OnLastPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 10, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnFirstPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnMiddlePage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 5, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnLastPage_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 10, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_OnPageExceedingTotal_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 15, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Empty_ShouldReturnMetadataWithDefaults()
    {
        // Act
        var metadata = PaginationMetadata.Empty();

        // Assert
        metadata.TotalCount.Should().Be(0);
        metadata.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
        metadata.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
        metadata.TotalPages.Should().Be(0);
        metadata.HasPreviousPage.Should().BeFalse();
        metadata.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_WithSingleIte_ShouldHaveOnePage()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 1, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(1);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_WithExactlyOnePage_ShouldHaveNoNextPage()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 10, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(1);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Create_WithOneMoreThanPageSize_ShouldHaveTwoPages()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 11, pageNumber: 1, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(2);
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Theory]
    [InlineData(100, 1, 10, 10, false, true)]
    [InlineData(100, 5, 10, 10, true, true)]
    [InlineData(100, 10, 10, 10, true, false)]
    [InlineData(50, 1, 25, 2, false, true)]
    [InlineData(50, 2, 25, 2, true, false)]
    [InlineData(0, 1, 10, 0, false, false)]
    public void Create_WithVariousInputs_ShouldCalculateCorrectly(
        int totalCount,
        int pageNumber,
        int pageSize,
        int expectedTotalPages,
        bool expectedHasPrevious,
        bool expectedHasNext)
    {
        // Act
        var result = PaginationMetadata.Create(totalCount, pageNumber, pageSize);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(expectedTotalPages);
        result.Value.HasPreviousPage.Should().Be(expectedHasPrevious);
        result.Value.HasNextPage.Should().Be(expectedHasNext);
    }

    [Fact]
    public void Create_WithLargeDataset_ShouldCalculateCorrectly()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 10000, pageNumber: 50, pageSize: 100);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(10000);
        result.Value.PageNumber.Should().Be(50);
        result.Value.PageSize.Should().Be(100);
        result.Value.TotalPages.Should().Be(100);
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Create_OnSecondToLastPage_ShouldHaveBothPreviousAndNext()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 9, pageSize: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPages.Should().Be(10);
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }
}
