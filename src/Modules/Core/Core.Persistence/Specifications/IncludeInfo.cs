using System.Linq.Expressions;
using Core.Domain.Primitives;

namespace Core.Persistence.Specifications;

public sealed class IncludeInfo<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, object>> Include { get; }
    public Expression<Func<object, object>>? ThenInclude { get; }

    public IncludeInfo(Expression<Func<TEntity, object>> include, Expression<Func<object, object>>? thenInclude = null)
    {
        Include = include;
        ThenInclude = thenInclude;
    }
}
