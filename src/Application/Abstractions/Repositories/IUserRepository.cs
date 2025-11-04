using Domain.Users;

namespace Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByUsername(string username);
}
