using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class EmailErrors
{
    public static Error Empty => new(
        Code: "Email.Empty",
        Description: "Email cannot be empty.");

    public static Error TooLong => new(
        Code: "Email.TooLong",
        Description: $"Email cannot be longer than {Email.MaxLength} characters.");

    public static Error InvalidFormat => new(
        Code: "Email.InvalidFormat",
        Description: "Email format is invalid.");
}
