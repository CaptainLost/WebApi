using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class NicknameErrors
{
    public static Error Empty => new(
        Code: "Nickname.Empty",
        Description: "Nickname cannot be empty.");

    public static Error TooLong => new(
        Code: "Nickname.TooLong",
        Description: $"Nickname cannot be longer than {Nickname.MaxLength} characters.");
}
