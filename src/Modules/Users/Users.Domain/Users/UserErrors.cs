using Core.Domain.Messaging;

namespace Users.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(string username) => new(
        Code: "User.NotFound",
        Description: $"User with username '{username}' was not found.");

    public static Error UserNotFoundById(Guid userId) => new(
        Code: "User.NotFoundById",
        Description: $"User with id '{userId}' was not found.");

    public static Error AlreadyHasRole(string roleName) => new(
        Code: "User.AlreadyHasRole",
        Description: $"User already has role '{roleName}'.");

    public static Error UsernameAlreadyTaken(string username) => new(
        Code: "User.UsernameAlreadyTaken",
        Description: $"Username '{username}' is already taken.");

    public static Error EmailAlreadyTaken(string email) => new(
        Code: "User.EmailAlreadyTaken",
        Description: $"Email '{email}' is already taken.");

    public static Error InvalidPasswordHash => new(
        Code: "User.InvalidPasswordHash",
        Description: "Password hash cannot be empty.");

    public static Error InvalidCredentials => new(
        Code: "User.InvalidCredentials",
        Description: "The provided credentials are invalid.");

    public static Error AccountLockedOut => new(
        Code: "User.AccountLockedOut",
        Description: "The account is locked out due to multiple failed login attempts.");
}
