using Application.Abstractions.Repositories;
using Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Users.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly UserManager<User> m_userManager;
    private readonly SignInManager<User> m_signInManager;

    public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        m_userManager = userManager;
        m_signInManager = signInManager;
    }

    public async Task LoginUser()
    {
        var user = await m_userManager.FindByNameAsync("xd");
    }
}
