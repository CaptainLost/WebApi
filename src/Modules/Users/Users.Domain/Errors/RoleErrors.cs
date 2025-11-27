using Core.Domain.Messaging;

namespace Users.Domain.Errors;

public static class RoleErrors
{
    public static Error RoleNotFound(string roleName) => new(
         Code: "Role.RoleNotFound",
        Description: $"Role '{roleName}' does not exist.");
}
