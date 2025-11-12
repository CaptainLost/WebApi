namespace Core.Presentation.Models;

public sealed record PaginationQueryParameters(
    int? PageNumber = null,
    int? PageSize = null,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);
