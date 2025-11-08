using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Authentication.Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> m_userManager;
    private readonly SignInManager<User> m_signInManager;

    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        m_userManager = userManager;
        m_signInManager = signInManager;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        return await m_signInManager.UserManager.GetUserAsync(m_signInManager.Context.User);
    }

    public async Task<Result> LoginAsync(User user, string password, bool isPersistent)
    {
        SignInResult signInResult = await m_signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: isPersistent,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return AuthenticationErrors.AccountLockedOut();
        }

        if (!signInResult.Succeeded)
        {
            return AuthenticationErrors.LoginFailed();
        }

        return Result.Success();
    }

    public async Task LogoutAsync()
    {
        await m_signInManager.SignOutAsync();
    }

    public async Task<Result> RegisterAsync(string username, string email, string password)
    {
        User? existingUser = await m_userManager.FindByNameAsync(username);

        if (existingUser != null)
        {
            return AuthenticationErrors.UsernameAlreadyTaken();
        }

        User? existingEmail = await m_userManager.FindByEmailAsync(email);

        if (existingEmail != null)
        {
            return AuthenticationErrors.EmailAlreadyTaken();
        }

        User newUser = new()
        {
            UserName = username,
            Email = email
        };

        IdentityResult result = await m_userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return AuthenticationErrors.RegistrationFailed(errors);
        }

        return Result.Success();
    }
}
