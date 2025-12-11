using Core.Domain.Pagination;
using FluentAssertions;

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
        pageRequest.PageNumber.Should().Be(2);
        pageRequest.PageSize.Should().Be(20);
        pageRequest.SearchTerm.Should().Be("test");
        pageRequest.SortBy.Should().Be("Name");
        pageRequest.SortDescending.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNullPageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: null, pageSize: 10);

        // Assert
        pageRequest.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
    }

    [Fact]
    public void Create_WithNullPageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: null);

        // Assert
        pageRequest.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
    }

    [Fact]
    public void Create_WithNoParameters_ShouldUseDefaults()
    {
        // Act
        var pageRequest = PageRequest.Create();

        // Assert
        pageRequest.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
        pageRequest.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
        pageRequest.SearchTerm.Should().BeNull();
        pageRequest.SortBy.Should().BeNull();
        pageRequest.SortDescending.Should().BeFalse();
    }

    [Fact]
    public void Create_WithNegativePageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: -1, pageSize: 10);

        // Assert
        pageRequest.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
    }

    [Fact]
    public void Create_WithZeroPageNumber_ShouldUseDefaultPageNumber()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 0, pageSize: 10);

        // Assert
        pageRequest.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
    }

    [Fact]
    public void Create_WithNegativePageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: -5);

        // Assert
        pageRequest.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
    }

    [Fact]
    public void Create_WithZeroPageSize_ShouldUseDefaultPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 0);

        // Assert
        pageRequest.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
    }

    [Fact]
    public void Create_WithPageSizeGreaterThanMax_ShouldUseMaxPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 100);

        // Assert
        pageRequest.PageSize.Should().Be(PaginationConstants.MaxPageSize);
    }

    [Fact]
    public void Create_WithPageSizeEqualToMax_ShouldUseMaxPageSize()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: PaginationConstants.MaxPageSize);

        // Assert
        pageRequest.PageSize.Should().Be(PaginationConstants.MaxPageSize);
    }

    [Fact]
    public void Create_WithWhitespaceSearchTer_ShouldSetSearchTermToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: "   ");

        // Assert
        pageRequest.SearchTerm.Should().BeNull();
    }

    [Fact]
    public void Create_WithSearchTermWithSpaces_ShouldTrimSearchTerm()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: "  test  ");

        // Assert
        pageRequest.SearchTerm.Should().Be("test");
    }

    [Fact]
    public void Create_WithEmptySearchTer_ShouldSetSearchTermToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, searchTerm: string.Empty);

        // Assert
        pageRequest.SearchTerm.Should().BeNull();
    }

    [Fact]
    public void Create_WithWhitespaceSortBy_ShouldSetSortByToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: "   ");

        // Assert
        pageRequest.SortBy.Should().BeNull();
    }

    [Fact]
    public void Create_WithSortByWithSpaces_ShouldTrimSortBy()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: "  Name  ");

        // Assert
        pageRequest.SortBy.Should().Be("Name");
    }

    [Fact]
    public void Create_WithEmptySortBy_ShouldSetSortByToNull()
    {
        // Act
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10, sortBy: string.Empty);

        // Assert
        pageRequest.SortBy.Should().BeNull();
    }

    [Fact]
    public void CalculateSkip_WithFirstPage_ShouldReturnZero()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 1, pageSize: 10);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        skip.Should().Be(0);
    }

    [Fact]
    public void CalculateSkip_WithSecondPage_ShouldReturnPageSize()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 2, pageSize: 10);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        skip.Should().Be(10);
    }

    [Fact]
    public void CalculateSkip_WithThirdPage_ShouldReturnTwicePageSize()
    {
        // Arrange
        var pageRequest = PageRequest.Create(pageNumber: 3, pageSize: 15);

        // Act
        var skip = pageRequest.CalculateSkip();

        // Assert
        skip.Should().Be(30);
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
        skip1.Should().Be(80); // (5-1) * 20 = 80
        skip2.Should().Be(45); // (10-1) * 5 = 45
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
        pageRequest.PageNumber.Should().Be(PaginationConstants.DefaultPageNumber);
        pageRequest.PageSize.Should().Be(PaginationConstants.DefaultPageSize);
        pageRequest.SearchTerm.Should().BeNull();
        pageRequest.SortBy.Should().BeNull();
        pageRequest.SortDescending.Should().BeFalse();
    }

    [Fact]
    public void Create_SortDescending_ShouldBeSetCorrectly()
    {
        // Act
        var pageRequestTrue = PageRequest.Create(sortDescending: true);
        var pageRequestFalse = PageRequest.Create(sortDescending: false);

        // Assert
        pageRequestTrue.SortDescending.Should().BeTrue();
        pageRequestFalse.SortDescending.Should().BeFalse();
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
        skip.Should().Be(expectedSkip);
    }
}
