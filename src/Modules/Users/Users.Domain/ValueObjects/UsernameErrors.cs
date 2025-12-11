using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class UsernameErrors
{
    public static Error Empty => Error.Validation(
        "Username.Empty",
        "Username cannot be empty.");

    public static Error TooLong => Error.Validation(
        "Username.TooLong",
        $"Username cannot be longer than {Username.MaxLength} characters.");
}
