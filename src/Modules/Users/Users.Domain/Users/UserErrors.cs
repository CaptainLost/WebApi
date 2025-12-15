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

    public static Error InvalidCredentials => Error.Authorization(
        "User.InvalidCredentials",
        "The provided credentials are invalid.");

    public static Error AccountLockedOut => Error.Authorization(
        "User.AccountLockedOut",
        "The account is locked out due to multiple failed login attempts.");

    public static Error RegistrationFailed => Error.Failure(
        "User.RegistrationFailed",
        "Registration failed. Please try again.");

    public static Error NotBanned => Error.Conflict(
        "User.NotBanned",
        "User is not currently banned.");

    public static Error BanReasonRequired => Error.Validation(
        "User.BanReasonRequired",
        "Ban reason is required.");

    public static Error BanExpirationMustBeInFuture => Error.Validation(
        "User.BanExpirationMustBeInFuture",
        "Ban expiration date must be in the future.");

    public static Error BannedByRequired => Error.Validation(
        "User.BannedByRequired",
        "BannedBy user ID is required.");

    public static Error UserIsBanned => Error.Authorization(
        "User.UserIsBanned",
        "The account is banned.");
}
