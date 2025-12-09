using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const string Name = "Email";
    public const int MaxLength = 255;

    private Email(string value) => Value = value;

    private Email()
    {
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<Email> Create(string email)
    {
        return Result.Create(email)
            .Ensure(
                e => !string.IsNullOrWhiteSpace(e),
                EmailErrors.Empty)
            .Ensure(
                e => e.Length <= MaxLength,
                EmailErrors.TooLong)
            .Ensure(
                e => e.Split('@').Length == 2
                     && !string.IsNullOrWhiteSpace(e.Split('@')[0])
                     && !string.IsNullOrWhiteSpace(e.Split('@')[1]),
                EmailErrors.InvalidFormat)
            .Map(e => new Email(e));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}