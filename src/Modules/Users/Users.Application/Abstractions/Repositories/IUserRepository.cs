using Core.Domain.Entities;
using Core.Domain.Messaging;
using Core.Domain.Pagination;

namespace Users.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetUsersPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
}
