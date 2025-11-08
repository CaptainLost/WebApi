using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly UserManager<User> m_userManager;

    public UserRepository(UserManager<User> userManager)
    {
        m_userManager = userManager;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await m_userManager.FindByNameAsync(username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await m_userManager.FindByEmailAsync(email);
    }
}
