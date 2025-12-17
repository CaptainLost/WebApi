using Core.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> inputQueryable,
        Specification<TEntity> specification)
        where TEntity : Entity
    {
        IQueryable<TEntity> queryable = inputQueryable;

        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        foreach (IncludeInfo<TEntity> includeInfo in specification.Includes)
        {
            if (includeInfo.ThenInclude is not null)
            {
                queryable = queryable
                    .Include(includeInfo.Include)
                    .ThenInclude(includeInfo.ThenInclude);
            }
            else
            {
                queryable = queryable.Include(includeInfo.Include);
            }
        }

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(
                specification.OrderByDescendingExpression);
        }

        if (specification.IsSplitQuery)
        {
            queryable = queryable.AsSplitQuery();
        }

        return queryable;
    }
}
