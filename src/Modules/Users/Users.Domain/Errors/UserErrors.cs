using Core.Domain.Messaging;

namespace Users.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(string username) => new(
        Code: "User.NotFound",
        Description: $"User with username '{username}' was not found.");

    public static Error UserNotFoundById(string userId) => new(
        Code: "User.NotFoundById",
        Description: $"User with id '{userId}' was not found.");

    public static Error UserAlreadyHasRole(string roleName) => new(
        Code: "User.AlreadyHasRole",
        Description: $"User already has role '{roleName}'.");

    public static Error AlreadyHasRole(string roleName) => new(
        Code: "User.AlreadyHasRole",
        Description: $"User already has role '{roleName}'.");
}
