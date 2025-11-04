using Domain.Messaging;
using Domain.Users;

namespace Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<Result> LoginAsync(User user, string password);
    Task<Result> RegisterAsync(string username, string email, string password);
}
