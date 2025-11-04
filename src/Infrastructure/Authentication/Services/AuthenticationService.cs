using Application.Abstractions.Services;
using Domain.Messaging;
using Domain.Users;
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

    public async Task<Result> LoginAsync(User user, string password)
    {
        SignInResult signInResult = await m_signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            return UserErrors.LoginFailed();
        }

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(string username, string email, string password)
    {
        User? existingUser = await m_userManager.FindByNameAsync(username);

        if (existingUser != null)
        {
            return UserErrors.UserAlreadyExists();
        }

        User? existingEmail = await m_userManager.FindByEmailAsync(email);

        if (existingEmail != null)
        {
            return UserErrors.UserAlreadyExists();
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
            return Result.Failure(new Error(
                "Authentication.RegistrationFailed",
                $"Registration failed: {errors}"));
        }

        return Result.Success();
    }
}
