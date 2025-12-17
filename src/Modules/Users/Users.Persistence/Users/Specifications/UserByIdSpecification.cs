using Users.Domain.Users;

namespace Users.Persistence.Users.Specifications;

internal sealed class UserByIdSpecification : Specification<User>
{
    public UserByIdSpecification(Guid id)
        : base(user => user.Id == id)
    {
        AddInclude(user => user.Roles);
    }
}
