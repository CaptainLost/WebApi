using Application.Abstractions.Repositories;
using Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Users.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly UserManager<User> m_userManager;

    public UserRepository(UserManager<User> userManager)
    {
        m_userManager = userManager;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await m_userManager.FindByNameAsync(username);
    }
}
