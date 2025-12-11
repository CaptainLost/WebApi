using Core.Domain.Pagination;

namespace Core.UnitTests.Domain.Pagination;

public sealed class PageRequestTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePageRequest()
    {
        // Act
        var pageRequest = PageRequest.Create(
            pageNumber: 2,
            pageSize: 20,
            searchTerm: "test",
            sortBy: "Name",
            sortDescending: true);

        // Assert
        Assert.Equal(2, pageRequest.PageNumber);
        Assert.Equal(20, pageRequest.PageSize);
        Assert.Equal("test", pageRequest.SearchTerm);
        Assert.Equal("Name", pageRequest.SortBy);
        Assert.True(pageRequest.SortDescending);
    }

    [Fact]
    public void Create_WithNullPageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: null, pageSize: 10);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageNumber, pageRequest.PageNumber);
    }

    [Fact]
    public void Create_WithNullPageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: null);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageSize, pageRequest.PageSize);
    }

    [Fact]
    public void Create_WithNoParameters_ShouldUseDefaults()
    {
        // Act
        var pageRequest = PageRequest.Create();

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageNumber, pageRequest.PageNumber);
        Assert.Equal(PaginationConstants.DefaultPageSize, pageRequest.PageSize);
        Assert.Null(pageRequest.SearchTerm);
        Assert.Null(pageRequest.SortBy);
        Assert.False(pageRequest.SortDescending);
    }

    [Fact]
    public void Create_WithNegativePageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: -1, pageSize: 10);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageNumber, pageRequest.PageNumber);
    }

    [Fact]
    public void Create_WithZeroPageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 0, pageSize: 10);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageNumber, pageRequest.PageNumber);
    }

    [Fact]
    public void Create_WithNegativePageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: -5);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageSize, pageRequest.PageSize);
    }

    [Fact]
    public void Create_WithZeroPageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 0);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageSize, pageRequest.PageSize);
    }

    [Fact]
    public void Create_WithPageSizeGreaterThanMax_ShouldUseMaxPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 100);

        // Assert
        Assert.Equal(PaginationConstants.MaxPageSize, pageRequest.PageSize);
    }

    [Fact]
    public void Create_WithPageSizeEqualToMax_ShouldUseMaxPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: PaginationConstants.MaxPageSize);

        // Assert
        Assert.Equal(PaginationConstants.MaxPageSize, pageRequest.PageSize);
    }

    [Fact]
    public void Create_WithWhitespaceSearchTer_ShouldSetSearchTermToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: "   ");

        // Assert
        Assert.Null(pageRequest.SearchTerm);
    }

    [Fact]
    public void Create_WithSearchTermWithSpaces_ShouldTrimSearchTerm()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: "  test  ");

        // Assert
        Assert.Equal("test", pageRequest.SearchTerm);
    }

    [Fact]
    public void Create_WithEmptySearchTer_ShouldSetSearchTermToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: string.Empty);

        // Assert
        Assert.Null(pageRequest.SearchTerm);
    }

    [Fact]
    public void Create_WithWhitespaceSortBy_ShouldSetSortByToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: "   ");

        // Assert
        Assert.Null(pageRequest.SortBy);
    }

    [Fact]
    public void Create_WithSortByWithSpaces_ShouldTrimSortBy()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: "  Name  ");

        // Assert
        Assert.Equal("Name", pageRequest.SortBy);
    }

    [Fact]
    public void Create_WithEmptySortBy_ShouldSetSortByToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: string.Empty);

        // Assert
        Assert.Null(pageRequest.SortBy);
    }

    [Fact]
    public void CalculateSkip_WithFirstPage_ShouldReturnZero()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        Assert.Equal(0, skip);
    }

    [Fact]
    public void CalculateSkip_WithSecondPage_ShouldReturnPageSize()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 2, pageSize: 10);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        Assert.Equal(10, skip);
    }

    [Fact]
    public void CalculateSkip_WithThirdPage_ShouldReturnTwicePageSize()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 3, pageSize: 15);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        Assert.Equal(30, skip);
    }

    [Fact]
    public void CalculateSkip_WithDifferentPageSizes_ShouldCalculateCorrectly()
    {
        // Arrange
        var pageRequest1 = PageRequest.Create(pageNumber: 5, pageSize: 20);
        var pageRequest2 = PageRequest.Create(pageNumber: 10, pageSize: 5);

        // Act
        var skip1 = pageRequest1.CalculateSkip();
        var skip2 = pageRequest2.CalculateSkip();

        // Assert
        Assert.Equal(80, skip1); // (5-1) * 20 = 80
        Assert.Equal(45, skip2); // (10-1) * 5 = 45
    }

    [Fact]
    public void Create_WithAllNullOptionalParameters_ShouldSetDefaultsAndNulls()
    {
        // Act
        var pageRequest = PageRequest.Create(
            pageNumber: null,
            pageSize: null,
            searchTerm: null,
            sortBy: null,
            sortDescending: false);

        // Assert
        Assert.Equal(PaginationConstants.DefaultPageNumber, pageRequest.PageNumber);
        Assert.Equal(PaginationConstants.DefaultPageSize, pageRequest.PageSize);
        Assert.Null(pageRequest.SearchTerm);
        Assert.Null(pageRequest.SortBy);
        Assert.False(pageRequest.SortDescending);
    }

    [Fact]
    public void Create_SortDescending_ShouldBeSetCorrectly()
    {
        // Act
        var pageRequestTrue = PageRequest.Create(sortDescending: true);
        var pageRequestFalse = PageRequest.Create(sortDescending: false);

        // Assert
        Assert.True(pageRequestTrue.SortDescending);
        Assert.False(pageRequestFalse.SortDescending);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 10, 20)]
    [InlineData(1, 25, 0)]
    [InlineData(5, 25, 100)]
    public void CalculateSkip_WithVariousInputs_ShouldReturnCorrectValue(int pageNumber, int pageSize, int expectedSkip)
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: pageNumber, pageSize: pageSize);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        Assert.Equal(expectedSkip, skip);
    }
}
