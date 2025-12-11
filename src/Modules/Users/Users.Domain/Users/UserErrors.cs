using Core.Domain.Messaging;

namespace Users.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(string username) => Error.NotFound(
        "User.NotFound",
        $"User with username '{username}' was not found.");

    public static Error UserNotFoundById(Guid userId) => Error.NotFound(
        "User.NotFoundById",
        $"User with id '{userId}' was not found.");

    public static Error AlreadyHasRole(string roleName) => Error.Conflict(
        "User.AlreadyHasRole",
        $"User already has role '{roleName}'.");

    public static Error UsernameAlreadyTaken(string username) => Error.Conflict(
        "User.UsernameAlreadyTaken",
        $"Username '{username}' is already taken.");

    public static Error EmailAlreadyTaken(string email) => Error.Conflict(
        "User.EmailAlreadyTaken",
        $"Email '{email}' is already taken.");

    public static Error InvalidPasswordHash => Error.Validation(
        "User.InvalidPasswordHash",
        "Password hash cannot be empty.");

    public static Error InvalidCredentials => Error.Authorization(
        "User.InvalidCredentials",
        "The provided credentials are invalid.");

    public static Error AccountLockedOut => Error.Authorization(
        "User.AccountLockedOut",
        "The account is locked out due to multiple failed login attempts.");
}
