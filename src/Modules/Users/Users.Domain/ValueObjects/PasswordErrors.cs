using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class PasswordErrors
{
    public static Error EmptyHash => Error.Failure(
        "Password.EmptyHash",
        "Password hash cannot be empty.");

    public static Error HashTooShort => Error.Failure(
        "Password.HashTooShort",
        $"Password hash must be at least {Password.MinHashLength} characters long.");

    public static Error EmptySalt => Error.Failure(
        "Password.EmptySalt",
        "Password salt cannot be empty.");

    public static Error SaltTooShort => Error.Failure(
        "Password.SaltTooShort",
        $"Password salt must be at least {Password.MinSaltLength} bytes long.");
}
