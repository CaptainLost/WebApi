using Core.Domain.Messaging;
using Core.Domain.Pagination;
using Users.Domain.Entities;

namespace Users.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdWithRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetUsersPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);

    Task<HashSet<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
}
