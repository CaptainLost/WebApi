using Users.Domain.Entities;
using Core.Domain.Messaging;

namespace Users.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<User?> GetCurrentUserAsync();
    Task<string?> GetCurrentUserIdAsync();
    Task<Result> LoginAsync(User user, string password, bool isPersistent);
    Task LogoutAsync();
    Task<Result> RegisterAsync(string username, string email, string password);
}