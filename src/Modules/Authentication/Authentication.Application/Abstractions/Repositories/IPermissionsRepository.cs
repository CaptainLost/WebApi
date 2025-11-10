using Core.Domain.Entities;

namespace Authentication.Application.Abstractions.Repositories;

public interface IPermissionsRepository
{
    Task<IReadOnlyCollection<Role>> GetUserRolesWithPermissionsAsync(string userId, CancellationToken cancellationToken = default);
}
