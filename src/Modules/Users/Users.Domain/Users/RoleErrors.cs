using Core.Domain.Messaging;

namespace Users.Domain.Users;

public static class RoleErrors
{
    public static Error NotFound(string roleName) => new(
        Code: "Role.NotFound",
        Description: $"Role '{roleName}' not found.");
}
