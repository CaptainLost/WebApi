using Authentication.Application.Abstractions.Repositories;
using Authentication.Application.Abstractions.Services;

namespace Authentication.Infrastructure.Authorization;

internal sealed class PermissionService : IPermissionService
{
    private readonly IPermissionsRepository m_permissionsRepository;

    public PermissionService(IPermissionsRepository permissionsRepository)
    {
        m_permissionsRepository = permissionsRepository;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(string userId)
    {
        IReadOnlyCollection<Core.Domain.Entities.Role> roles =
            await m_permissionsRepository.GetUserRolesWithPermissionsAsync(userId);

        return roles
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
