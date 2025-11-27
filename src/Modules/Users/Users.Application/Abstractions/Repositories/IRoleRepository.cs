using Users.Domain.Entities;

namespace Users.Application.Abstractions.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
}
