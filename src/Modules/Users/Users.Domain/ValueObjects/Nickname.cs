using Core.Domain.Errors;
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
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return Result.Failure<Nickname>(DomainErrors.IsEmpty(Name));
        }

        if (nickname.Length > MaxLength)
        {
            return Result.Failure<Nickname>(DomainErrors.TooLong(Name));
        }

        return new Nickname(nickname);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
