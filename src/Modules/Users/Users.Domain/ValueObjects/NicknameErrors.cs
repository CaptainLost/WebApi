using Core.Domain.Messaging;

namespace Users.Domain.ValueObjects;

public static class NicknameErrors
{
    public static Error Empty => Error.Validation(
        "Nickname.Empty",
        "Nickname cannot be empty.");

    public static Error TooLong => Error.Validation(
        "Nickname.TooLong",
        $"Nickname cannot be longer than {Nickname.MaxLength} characters.");
}
