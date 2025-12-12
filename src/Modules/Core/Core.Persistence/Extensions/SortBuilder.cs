using System.Linq.Expressions;

namespace Core.Persistence.Extensions;

public sealed class SortBuilder<T>
{
    private readonly IQueryable<T> _query;
    private readonly string? _sortBy;
    private readonly bool _sortDescending;
    private readonly Dictionary<string, Expression<Func<T, object>>> _sortMappings = new();
    private Expression<Func<T, object>>? _defaultSort;

    internal SortBuilder(IQueryable<T> query, string? sortBy, bool sortDescending)
    {
        _query = query;
        _sortBy = sortBy;
        _sortDescending = sortDescending;
    }

    public SortBuilder<T> By(string key, Expression<Func<T, object>> selector)
    {
        _sortMappings[key.ToLowerInvariant()] = selector;

        return this;
    }

    public IQueryable<T> WithDefault(Expression<Func<T, object>> selector)
    {
        _defaultSort = selector;

        return Apply();
    }

    private IQueryable<T> Apply()
    {
        Expression<Func<T, object>>? sortProperty = null;

        if (!string.IsNullOrWhiteSpace(_sortBy) &&
            _sortMappings.TryGetValue(_sortBy.ToLowerInvariant(), out Expression<Func<T, object>>? property))
        {
            sortProperty = property;
        }
        else
        {
            sortProperty = _defaultSort;
        }

        if (sortProperty == null)
        {
            return _query;
        }

        return _sortDescending
            ? _query.OrderByDescending(sortProperty)
            : _query.OrderBy(sortProperty);
    }
}
