using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Abstractions;

public interface IAuthenticationService
{
    Task<Result<string>> LoginAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<Result<string>> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);
}