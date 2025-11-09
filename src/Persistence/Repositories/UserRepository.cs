using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

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
}
