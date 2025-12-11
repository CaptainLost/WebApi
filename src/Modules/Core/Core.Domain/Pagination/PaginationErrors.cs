using Core.Domain.Messaging;

namespace Core.Domain.Pagination;

public static class PaginationErrors
{
    public static readonly Error NegativeTotalCount = Error.Validation(
        "Pagination.NegativeTotalCount",
        "Total count cannot be negative");

    public static readonly Error InvalidPageNumber = Error.Validation(
        "Pagination.InvalidPageNumber",
        "Page number must be greater than zero");

    public static readonly Error InvalidPageSize = Error.Validation(
        "Pagination.InvalidPageSize",
        "Page size must be greater than zero");
}
