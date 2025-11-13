using Core.Domain.Errors;
using Core.Domain.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public sealed class User : IdentityUser
{
    public ICollection<Role> Roles { get; private set; } = [];

    public bool HasRole(string roleName)
    {
        return Roles.Any(r => r.Name == roleName);
    }

    public Result AssignRole(Role role)
    {
        if (HasRole(role.Name))
        {
            return UserErrors.AlreadyHasRole(role.Name);
        }

        Roles.Add(role);
        return Result.Success();
    }
}