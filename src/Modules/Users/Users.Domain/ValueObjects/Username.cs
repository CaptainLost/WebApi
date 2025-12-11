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
        if (username is null)
        {
            return Result.Failure<Username>(UsernameErrors.Empty);
        }

        return Result.Create(username)
            .Ensure(
                u => !string.IsNullOrWhiteSpace(u),
                UsernameErrors.Empty)
            .Ensure(
                u => u.Length <= MaxLength,
                UsernameErrors.TooLong)
            .Map(u => new Username(u));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
