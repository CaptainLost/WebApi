using Users.Domain.Users;

namespace Users.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
