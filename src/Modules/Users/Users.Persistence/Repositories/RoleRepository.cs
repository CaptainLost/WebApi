using Core.Domain.Entities;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Repositories;

namespace Users.Persistence.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext m_dbContext;

    public RoleRepository(ApplicationDbContext dbContext)
    {
        m_dbContext = dbContext;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await m_dbContext.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        return await m_dbContext.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .AnyAsync(r => r.Name == roleName, cancellationToken);
    }
}
