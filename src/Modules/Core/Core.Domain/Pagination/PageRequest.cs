namespace Core.Domain.Pagination;

public sealed record PageRequest
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public string? SearchTerm { get; }
    public string? SortBy { get; }
    public bool SortDescending { get; }

    private PageRequest(int pageNumber, int pageSize, string? searchTerm, string? sortBy, bool sortDescending)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        SortBy = sortBy;
        SortDescending = sortDescending;
    }

    public static PageRequest Create(
        int? pageNumber = null,
        int? pageSize = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        int validatedPageNumber = ValidatePageNumber(pageNumber);
        int validatedPageSize = ValidatePageSize(pageSize);

        return new PageRequest(
            validatedPageNumber,
            validatedPageSize,
            string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
            string.IsNullOrWhiteSpace(sortBy) ? null : sortBy.Trim(),
            sortDescending);
    }

    private static int ValidatePageNumber(int? pageNumber)
    {
        if (pageNumber == null || pageNumber < 1)
        {
            return PaginationConstants.DefaultPageNumber;
        }

        return pageNumber.Value;
    }

    private static int ValidatePageSize(int? pageSize)
    {
        if (pageSize == null || pageSize < 1)
        {
            return PaginationConstants.DefaultPageSize;
        }

        if (pageSize.Value > PaginationConstants.MaxPageSize)
        {
            return PaginationConstants.MaxPageSize;
        }

        return pageSize.Value;
    }

    public int CalculateSkip() => (PageNumber - 1) * PageSize;
}
