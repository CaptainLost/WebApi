using Core.Domain.Messaging;
using Core.Domain.Pagination;

namespace Core.UnitTests.Domain.Pagination;

public sealed class PaginationMetadataTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePaginationMetadata()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 2, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value.TotalCount);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(10, result.Value.TotalPages);
    }

    [Fact]
    public void Create_WithNegativeTotalCount_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: -1, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PaginationErrors.NegativeTotalCount, result.Error);
    }

    [Fact]
    public void Create_WithZeroPageNumber_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 0, pageSize: 10);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PaginationErrors.InvalidPageNumber, result.Error);
    }

    [Fact]
    public void Create_WithNegativePageNumber_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: -1, pageSize: 10);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PaginationErrors.InvalidPageNumber, result.Error);
    }

    [Fact]
    public void Create_WithZeroPageSize_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 0);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PaginationErrors.InvalidPageSize, result.Error);
    }

    [Fact]
    public void Create_WithNegativePageSize_ShouldReturnFailure()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: -5);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PaginationErrors.InvalidPageSize, result.Error);
    }

    [Fact]
    public void TotalPages_WithEvenDivision_ShouldCalculateCorrectly()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.TotalPages);
    }

    [Fact]
    public void TotalPages_WithUnevenDivision_ShouldRoundUp()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 105, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(11, result.Value.TotalPages);
    }

    [Fact]
    public void TotalPages_WithZeroTotalCount_ShouldReturnZero()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 0, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.TotalPages);
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 2, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_OnLastPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 10, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_OnFirstPage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasNextPage);
    }

    [Fact]
    public void HasNextPage_OnMiddlePage_ShouldBeTrue()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 5, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasNextPage);
    }

    [Fact]
    public void HasNextPage_OnLastPage_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 10, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public void HasNextPage_OnPageExceedingTotal_ShouldBeFalse()
    {
        // Arrange
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 15, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public void Empty_ShouldReturnMetadataWithDefaults()
    {
        // Act
        var metadata = PaginationMetadata.Empty();

        // Assert
        Assert.Equal(0, metadata.TotalCount);
        Assert.Equal(PaginationConstants.DefaultPageNumber, metadata.PageNumber);
        Assert.Equal(PaginationConstants.DefaultPageSize, metadata.PageSize);
        Assert.Equal(0, metadata.TotalPages);
        Assert.False(metadata.HasPreviousPage);
        Assert.False(metadata.HasNextPage);
    }

    [Fact]
    public void Create_WithSingleIte_ShouldHaveOnePage()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 1, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalPages);
        Assert.False(result.Value.HasPreviousPage);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public void Create_WithExactlyOnePage_ShouldHaveNoNextPage()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 10, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalPages);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public void Create_WithOneMoreThanPageSize_ShouldHaveTwoPages()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 11, pageNumber: 1, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalPages);
        Assert.True(result.Value.HasNextPage);
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
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTotalPages, result.Value.TotalPages);
        Assert.Equal(expectedHasPrevious, result.Value.HasPreviousPage);
        Assert.Equal(expectedHasNext, result.Value.HasNextPage);
    }

    [Fact]
    public void Create_WithLargeDataset_ShouldCalculateCorrectly()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 10000, pageNumber: 50, pageSize: 100);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10000, result.Value.TotalCount);
        Assert.Equal(50, result.Value.PageNumber);
        Assert.Equal(100, result.Value.PageSize);
        Assert.Equal(100, result.Value.TotalPages);
        Assert.True(result.Value.HasPreviousPage);
        Assert.True(result.Value.HasNextPage);
    }

    [Fact]
    public void Create_OnSecondToLastPage_ShouldHaveBothPreviousAndNext()
    {
        // Act
        var result = PaginationMetadata.Create(totalCount: 100, pageNumber: 9, pageSize: 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.TotalPages);
        Assert.True(result.Value.HasPreviousPage);
        Assert.True(result.Value.HasNextPage);
    }
}
