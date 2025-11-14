using System.Linq.Expressions;
using System.Reflection;

namespace Core.Persistence.Extensions;

public static class QueryableExtensions
{
    public static SearchBuilder<T> Search<T>(
        this IQueryable<T> query,
        string? searchTerm)
    {
        return new SearchBuilder<T>(query, searchTerm);
    }

    public static SortBuilder<T> Sort<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDescending)
    {
        return new SortBuilder<T>(query, sortBy, sortDescending);
    }

    internal static Expression<Func<T, bool>> BuildContainsExpression<T>(
        Expression<Func<T, string>> propertyExpression,
        string searchTerm)
    {
        ParameterExpression parameter = propertyExpression.Parameters[0];
        Expression body = propertyExpression.Body;

        if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            body = unary.Operand;
        }

        MemberExpression? memberExpression = body as MemberExpression;
        if (memberExpression == null)
        {
            throw new ArgumentException("Expression must be a property access", nameof(propertyExpression));
        }

        BinaryExpression nullCheck = Expression.NotEqual(memberExpression, Expression.Constant(null, memberExpression.Type));

        MethodInfo containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        MethodCallExpression containsCall = Expression.Call(
            memberExpression,
            containsMethod,
            Expression.Constant(searchTerm));

        BinaryExpression combined = Expression.AndAlso(nullCheck, containsCall);

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    internal static Expression<Func<T, bool>> CombineOr<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        ParameterExpression parameter = left.Parameters[0];
        Expression body = Expression.OrElse(left.Body, Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
