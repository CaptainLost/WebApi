using Application.Abstractions.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Authentication.Authorization;

internal sealed class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext m_context;

    public PermissionService(
        ApplicationDbContext context)
    {
        m_context = context;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string userId)
    {
        ICollection<Role>[] roles = await m_context.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Where(x => x.Id == userId)
            .Select(x => x.Roles)
            .ToArrayAsync();

        return roles
            .SelectMany(x => x)
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
