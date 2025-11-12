using Core.Domain.Entities;
using Core.Domain.Messaging;
using Core.Domain.Pagination;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Repositories;

namespace Users.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext m_dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        m_dbContext = dbContext;
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        string normalizedUsername = username.ToUpperInvariant();

        return await m_dbContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUsername, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalizedEmail = email.ToUpperInvariant();

        return await m_dbContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        User? user = await m_dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return Array.Empty<string>();
        }

        return user.Roles.Select(r => r.Name).ToList();
    }

    public async Task<PagedResult<User>> GetUsersPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = m_dbContext.Users.AsQueryable();

        query = ApplyFiltering(query, pageRequest);
        query = ApplySorting(query, pageRequest);

        int totalCount = await query.CountAsync(cancellationToken);

        List<User> users = await query
            .Skip(pageRequest.CalculateSkip())
            .Take(pageRequest.PageSize)
            .ToListAsync(cancellationToken);

        Result<PagedResult<User>> pagedResult = PagedResult<User>.Create(users, totalCount, pageRequest.PageNumber, pageRequest.PageSize);

        if (pagedResult.IsFailure)
        {
            return PagedResult<User>.Empty();
        }

        return pagedResult.Value;
    }

    private static IQueryable<User> ApplyFiltering(IQueryable<User> query, PageRequest pageRequest)
    {
        if (string.IsNullOrWhiteSpace(pageRequest.SearchTerm))
        {
            return query;
        }

        string searchTermUpper = pageRequest.SearchTerm.ToUpperInvariant();

        return query.Where(u => 
            u.NormalizedUserName != null && u.NormalizedUserName.Contains(searchTermUpper));
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, PageRequest pageRequest)
    {
        if (string.IsNullOrWhiteSpace(pageRequest.SortBy))
        {
            return query.OrderBy(u => u.UserName);
        }

        return pageRequest.SortBy.ToLowerInvariant() switch
        {
            "username" => pageRequest.SortDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
            "email" => pageRequest.SortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            _ => query.OrderBy(u => u.UserName)
        };
    }
}
