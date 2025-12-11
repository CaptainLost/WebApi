using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.ValueObjects;

public sealed class Nickname : ValueObject
{
    public const string Name = "Nickname";
    public const int MaxLength = 50;

    private Nickname(string value)
    {
        Value = value;
    }

    private Nickname()
    {
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<Nickname> Create(string nickname)
    {
        if (nickname is null)
        {
            return Result.Failure<Nickname>(NicknameErrors.Empty);
        }

        return Result.Create(nickname)
            .Ensure(
                n => !string.IsNullOrWhiteSpace(n),
                NicknameErrors.Empty)
            .Ensure(
                n => n.Length <= MaxLength,
                NicknameErrors.TooLong)
            .Map(n => new Nickname(n));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
