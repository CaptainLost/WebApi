using Core.Domain.Entities;

namespace Users.Application.Abstractions.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<bool> UserHasRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);
}
