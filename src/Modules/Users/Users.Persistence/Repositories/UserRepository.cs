using Core.Domain.Entities;
using Core.Domain.Messaging;
using Core.Domain.Pagination;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Repositories;
using Users.Persistence.Specifications;

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

    public async Task<User?> GetUserByIdWithRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await m_dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
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

        UserFillterSpecification specification = new UserFillterSpecification(pageRequest);
        query = specification.Apply(query);

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

    public async Task<Result> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        m_dbContext.Users.Update(user);
        await m_dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
