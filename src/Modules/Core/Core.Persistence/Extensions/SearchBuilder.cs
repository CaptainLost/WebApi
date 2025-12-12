using System.Linq.Expressions;

namespace Core.Persistence.Extensions;

public sealed class SearchBuilder<T>
{
    private readonly IQueryable<T> _query;
    private readonly string? _searchTerm;
    private readonly List<Expression<Func<T, string>>> _searchProperties = new();

    internal SearchBuilder(IQueryable<T> query, string? searchTerm)
    {
        _query = query;
        _searchTerm = searchTerm;
    }

    public SearchBuilder<T> By(Expression<Func<T, string>> property)
    {
        _searchProperties.Add(property);

        return this;
    }

    public SortBuilder<T> Sort(string? sortBy, bool sortDescending)
    {
        IQueryable<T> filteredQuery = Apply();

        return new SortBuilder<T>(filteredQuery, sortBy, sortDescending);
    }

    private IQueryable<T> Apply()
    {
        if (string.IsNullOrWhiteSpace(_searchTerm) || _searchProperties.Count == 0)
        {
            return _query;
        }

        string searchUpper = _searchTerm.ToUpperInvariant();
        Expression<Func<T, bool>>? combinedFilter = null;

        foreach (Expression<Func<T, string>> property in _searchProperties)
        {
            Expression<Func<T, bool>> filter = QueryableExtensions.BuildContainsExpression(property, searchUpper);
            combinedFilter = combinedFilter == null
                ? filter
                : QueryableExtensions.CombineOr(combinedFilter, filter);
        }

        return combinedFilter != null ? _query.Where(combinedFilter) : _query;
    }
}
