using System.Linq.Expressions;

namespace Core.Persistence.Extensions;

public sealed class SearchBuilder<T>
{
    private readonly IQueryable<T> m_query;
    private readonly string? m_searchTerm;
    private readonly List<Expression<Func<T, string>>> m_searchProperties = new();

    internal SearchBuilder(IQueryable<T> query, string? searchTerm)
    {
        m_query = query;
        m_searchTerm = searchTerm;
    }

    public SearchBuilder<T> By(Expression<Func<T, string>> property)
    {
        m_searchProperties.Add(property);

        return this;
    }

    public SortBuilder<T> Sort(string? sortBy, bool sortDescending)
    {
        IQueryable<T> filteredQuery = Apply();
        
        return new SortBuilder<T>(filteredQuery, sortBy, sortDescending);
    }

    private IQueryable<T> Apply()
    {
        if (string.IsNullOrWhiteSpace(m_searchTerm) || m_searchProperties.Count == 0)
        {
            return m_query;
        }

        string searchUpper = m_searchTerm.ToUpperInvariant();
        Expression<Func<T, bool>>? combinedFilter = null;

        foreach (Expression<Func<T, string>> property in m_searchProperties)
        {
            Expression<Func<T, bool>> filter = QueryableExtensions.BuildContainsExpression(property, searchUpper);
            combinedFilter = combinedFilter == null
                ? filter
                : QueryableExtensions.CombineOr(combinedFilter, filter);
        }

        return combinedFilter != null ? m_query.Where(combinedFilter) : m_query;
    }
}
