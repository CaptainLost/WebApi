namespace Core.Domain.Pagination;

using Core.Domain.Messaging;

public static class PaginationErrors
{
    public static readonly Error NegativeTotalCount = new(
        "Pagination.NegativeTotalCount",
        "Total count cannot be negative");

    public static readonly Error InvalidPageNumber = new(
        "Pagination.InvalidPageNumber",
        "Page number must be greater than zero");

    public static readonly Error InvalidPageSize = new(
        "Pagination.InvalidPageSize",
        "Page size must be greater than zero");
}
