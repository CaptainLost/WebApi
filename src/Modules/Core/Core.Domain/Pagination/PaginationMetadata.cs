using Core.Domain.Messaging;

namespace Core.Domain.Pagination;

public sealed record PaginationMetadata
{
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }

    private PaginationMetadata(int totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = CalculateTotalPages(totalCount, pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }

    public static PaginationMetadata Empty() => new PaginationMetadata(0, PaginationConstants.DefaultPageNumber, PaginationConstants.DefaultPageSize);

    public static Result<PaginationMetadata> Create(int totalCount, int pageNumber, int pageSize)
    {
        if (totalCount < 0)
        {
            return Result.Failure<PaginationMetadata>(PaginationErrors.NegativeTotalCount);
        }

        if (pageNumber < 1)
        {
            return Result.Failure<PaginationMetadata>(PaginationErrors.InvalidPageNumber);
        }

        if (pageSize < 1)
        {
            return Result.Failure<PaginationMetadata>(PaginationErrors.InvalidPageSize);
        }

        return Result.Success(new PaginationMetadata(totalCount, pageNumber, pageSize));
    }

    private static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (totalCount == 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
