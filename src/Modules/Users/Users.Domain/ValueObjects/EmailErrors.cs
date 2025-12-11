using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class EmailErrors
{
    public static Error Empty => Error.Validation(
        "Email.Empty",
        "Email cannot be empty.");

    public static Error TooLong => Error.Validation(
        "Email.TooLong",
        $"Email cannot be longer than {Email.MaxLength} characters.");

    public static Error InvalidFormat => Error.Validation(
        "Email.InvalidFormat",
        "Email format is invalid.");
}
