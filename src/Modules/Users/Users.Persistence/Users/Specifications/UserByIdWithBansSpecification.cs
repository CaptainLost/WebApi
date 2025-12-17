using Users.Domain.Users;

namespace Users.Persistence.Users.Specifications;

internal sealed class UserByIdWithBansSpecification : Specification<User>
{
    public UserByIdWithBansSpecification(Guid id)
        : base(user => user.Id == id)
    {
        AddInclude(user => user.Bans);
    }
}
