using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class UsernameErrors
{
    public static Error Empty => new(
        Code: "Username.Empty",
        Description: "Username cannot be empty.");

    public static Error TooLong => new(
        Code: "Username.TooLong",
        Description: $"Username cannot be longer than {Username.MaxLength} characters.");
}
