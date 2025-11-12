namespace Core.Domain.Pagination;

using Core.Domain.Messaging;

public sealed record PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public PaginationMetadata Metadata { get; }

    private PagedResult(IReadOnlyCollection<T> items, PaginationMetadata metadata)
    {
        Items = items;
        Metadata = metadata;
    }

    public static Result<PagedResult<T>> Create(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        if (items == null)
        {
            return Result.Failure<PagedResult<T>>(Error.NullValue);
        }

        Result<PaginationMetadata> metadataResult = PaginationMetadata.Create(totalCount, pageNumber, pageSize);
        
        if (metadataResult.IsFailure)
        {
            return Result.Failure<PagedResult<T>>(metadataResult.Error);
        }

        return Result.Success(new PagedResult<T>(items, metadataResult.Value));
    }

    public static PagedResult<T> Empty() => new PagedResult<T>(Array.Empty<T>(), PaginationMetadata.Empty());
}
