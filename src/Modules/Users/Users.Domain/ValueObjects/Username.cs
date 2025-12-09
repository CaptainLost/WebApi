using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.ValueObjects;

public sealed class Username : ValueObject
{
    public const string Name = "Username";
    public const int MaxLength = 50;

    private Username(string value)
    {
        Value = value;
    }

    private Username()
    {
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<Username> Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result.Failure<Username>(UsernameErrors.Empty);
        }

        if (username.Length > MaxLength)
        {
            return Result.Failure<Username>(UsernameErrors.TooLong);
        }

        return new Username(username);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
