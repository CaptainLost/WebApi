using Domain.Entities;

namespace Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
}
