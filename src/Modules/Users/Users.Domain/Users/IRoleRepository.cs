namespace Users.Domain.Users;

public interface IRoleRepository
{
    Task<Role?> GetByName(string name, CancellationToken cancellationToken = default);
}