using Users.Domain.Users;

namespace Users.Persistence.Users.Specifications;

internal sealed class UserByIdWithRolesPermissionsAndBansSpecification : Specification<User>
{
    internal UserByIdWithRolesPermissionsAndBansSpecification(Guid id)
        : base(user => user.Id == id)
    {
        AddInclude(user => user.Roles, role => ((Role)role).Permissions);
        AddInclude(user => user.Bans);
    }
}
