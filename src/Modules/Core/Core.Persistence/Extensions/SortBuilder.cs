using System.Linq.Expressions;

namespace Core.Persistence.Extensions;

public sealed class SortBuilder<T>
{
    private readonly IQueryable<T> m_query;
    private readonly string? m_sortBy;
    private readonly bool m_sortDescending;
    private readonly Dictionary<string, Expression<Func<T, object>>> m_sortMappings = new();
    private Expression<Func<T, object>>? m_defaultSort;

    internal SortBuilder(IQueryable<T> query, string? sortBy, bool sortDescending)
    {
        m_query = query;
        m_sortBy = sortBy;
        m_sortDescending = sortDescending;
    }

    public SortBuilder<T> By(string key, Expression<Func<T, object>> selector)
    {
        m_sortMappings[key.ToLowerInvariant()] = selector;
        
        return this;
    }

    public IQueryable<T> WithDefault(Expression<Func<T, object>> selector)
    {
        m_defaultSort = selector;

        return Apply();
    }

    private IQueryable<T> Apply()
    {
        Expression<Func<T, object>>? sortProperty = null;

        if (!string.IsNullOrWhiteSpace(m_sortBy) &&
            m_sortMappings.TryGetValue(m_sortBy.ToLowerInvariant(), out Expression<Func<T, object>>? property))
        {
            sortProperty = property;
        }
        else
        {
            sortProperty = m_defaultSort;
        }

        if (sortProperty == null)
        {
            return m_query;
        }

        return m_sortDescending
            ? m_query.OrderByDescending(sortProperty)
            : m_query.OrderBy(sortProperty);
    }
}
