using Core.Domain.Pagination;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Users;
using Users.Domain.ValueObjects;
using Users.Persistence.Database;

namespace Users.Persistence.Users;

internal sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(Username username, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Users
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _dbContext.Users;

        if (!string.IsNullOrWhiteSpace(pageRequest.SearchTerm))
        {
            string normalizedSearchTerm = pageRequest.SearchTerm.ToUpperInvariant();

            query = query.Where(u =>
                u.Username.Value.ToUpper().Contains(normalizedSearchTerm) ||
                (u.Email != null && u.Email.Value.ToUpper().Contains(normalizedSearchTerm)));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, pageRequest.SortBy, pageRequest.SortDescending);

        List<User> users = await query
            .Skip(pageRequest.CalculateSkip())
            .Take(pageRequest.PageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return sortDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username);
        }

        return sortBy.ToLowerInvariant() switch
        {
            "username" => sortDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
            "email" => sortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "name" => sortDescending ? query.OrderByDescending(u => u.Nickname) : query.OrderBy(u => u.Nickname),
            _ => sortDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username)
        };
    }
}