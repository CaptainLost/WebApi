using Users.Domain.Entities;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Repositories;
using Users.Persistence.Database;

namespace Users.Persistence.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly UsersDbContext m_dbContext;

    public RoleRepository(UsersDbContext dbContext)
    {
        m_dbContext = dbContext;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await m_dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }
}
