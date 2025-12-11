using Core.Domain.Messaging;

namespace Users.Domain.Users;

public static class RoleErrors
{
    public static Error NotFound(string roleName) => Error.NotFound(
        "Role.NotFound",
        $"Role '{roleName}' not found.");
}
