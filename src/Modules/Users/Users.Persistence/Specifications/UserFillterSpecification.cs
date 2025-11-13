using Core.Domain.Entities;
using Core.Domain.Pagination;

namespace Users.Persistence.Specifications;

internal sealed class UserFillterSpecification
{
    private readonly PageRequest m_pageRequest;

    public UserFillterSpecification(PageRequest pageRequest)
    {
        m_pageRequest = pageRequest;
    }

    public IQueryable<User> Apply(IQueryable<User> query)
    {
        query = ApplyFiltering(query);
        query = ApplySorting(query);

        return query;
    }

    private IQueryable<User> ApplyFiltering(IQueryable<User> query)
    {
        if (string.IsNullOrWhiteSpace(m_pageRequest.SearchTerm))
        {
            return query;
        }

        string searchTermUpper = m_pageRequest.SearchTerm.ToUpperInvariant();

        return query.Where(u => 
            u.NormalizedUserName != null && u.NormalizedUserName.Contains(searchTermUpper));
    }

    private IQueryable<User> ApplySorting(IQueryable<User> query)
    {
        if (string.IsNullOrWhiteSpace(m_pageRequest.SortBy))
        {
            return query.OrderBy(u => u.UserName);
        }

        return m_pageRequest.SortBy.ToLowerInvariant() switch
        {
            "username" => m_pageRequest.SortDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
            "email" => m_pageRequest.SortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            _ => query.OrderBy(u => u.UserName)
        };
    }
}
