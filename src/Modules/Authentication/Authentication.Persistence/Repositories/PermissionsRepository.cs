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

    public async Task<HashSet<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        return await m_context.Set<User>()
            .Where(x => x.Id == userId)
            .Include(x => x.Roles)
            .SelectMany(x => x.Roles)
            .Select(x => x.Name)
            .ToHashSetAsync(cancellationToken);
    }

    public async Task<HashSet<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        return await m_context.Set<User>()
            .Where(x => x.Id == userId)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .SelectMany(x => x.Roles)
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSetAsync(cancellationToken);
    }
}
