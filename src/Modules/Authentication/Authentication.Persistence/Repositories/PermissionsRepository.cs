using Authentication.Application.Abstractions.Repositories;
using Core.Domain.Entities;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistence.Repositories;

internal sealed class PermissionsRepository : IPermissionsRepository
{
    private readonly ApplicationDbContext m_context;

    public PermissionsRepository(ApplicationDbContext context)
    {
        m_context = context;
    }

    public async Task<IReadOnlyCollection<Role>> GetUserRolesWithPermissionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        ICollection<Role>[] roles = await m_context.Set<User>()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Where(x => x.Id == userId)
            .Select(x => x.Roles)
            .ToArrayAsync(cancellationToken);

        if (roles.Length == 0)
        {
            return Array.Empty<Role>();
        }

        return roles[0].ToList();
    }
}
