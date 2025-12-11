using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class PlainPasswordErrors
{
    public static Error Empty => Error.Validation(
        "PlainPassword.Empty",
        "Password cannot be empty.");

    public static Error TooShort => Error.Validation(
        "PlainPassword.TooShort",
        "Password must be at least 8 characters long.");

    public static Error TooLong => Error.Validation(
        "PlainPassword.TooLong",
        "Password must not exceed 128 characters.");

    public static Error MissingUppercase => Error.Validation(
        "PlainPassword.MissingUppercase",
        "Password must contain at least one uppercase letter.");

    public static Error MissingLowercase => Error.Validation(
        "PlainPassword.MissingLowercase",
        "Password must contain at least one lowercase letter.");

    public static Error MissingDigit => Error.Validation(
        "PlainPassword.MissingDigit",
        "Password must contain at least one digit.");

    public static Error MissingSpecialCharacter => Error.Validation(
        "PlainPassword.MissingSpecialCharacter",
        "Password must contain at least one special character.");
}
