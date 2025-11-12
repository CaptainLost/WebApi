namespace Authentication.Application.Abstractions.Repositories;

public interface IPermissionsRepository
{
    Task<HashSet<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<HashSet<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
}
