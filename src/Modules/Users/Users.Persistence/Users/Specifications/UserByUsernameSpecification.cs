using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.Persistence.Users.Specifications;

internal sealed class UserByUsernameSpecification : Specification<User>
{
    public UserByUsernameSpecification(Username username)
        : base(user => user.Username == username)
    {
        AddInclude(user => user.Roles);
        AddInclude(user => user.Bans);
    }
}
