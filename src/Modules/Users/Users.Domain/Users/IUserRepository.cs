using Core.Domain.Pagination;
using Users.Domain.ValueObjects;

namespace Users.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(Username username, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
    Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);

    void Add(User user);
    Task SaveChangesAsync();
}