using System.Linq.Expressions;
using Core.Domain.Primitives;
using Core.Persistence.Specifications;

public abstract class Specification<TEntity>
    where TEntity : Entity
{
    protected Specification(Expression<Func<TEntity, bool>>? criteria)
    {
        Criteria = criteria;
    }
    public bool IsSplitQuery { get; protected set; }

    public Expression<Func<TEntity, bool>>? Criteria { get; }

    public List<IncludeInfo<TEntity>> Includes { get; } = new();

    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        Includes.Add(new IncludeInfo<TEntity>(includeExpression));
    }

    protected void AddInclude(
        Expression<Func<TEntity, object>> includeExpression,
        Expression<Func<object, object>> thenIncludeExpression)
    {
        Includes.Add(new IncludeInfo<TEntity>(includeExpression, thenIncludeExpression));
    }

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        OrderByExpression = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        OrderByDescendingExpression = orderByDescendingExpression;
    }

    protected void AddSplitQuery()
    {
        IsSplitQuery = true;
    }
}