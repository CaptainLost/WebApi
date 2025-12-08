using Core.Domain.Errors;
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

    public static Result<Email> Create(string email) =>
        Result.Create(email)
            .Ensure(
                e => !string.IsNullOrWhiteSpace(e),
                DomainErrors.IsEmpty(Name))
            .Ensure(
                e => e.Length <= MaxLength,
                DomainErrors.TooLong(Name))
            .Ensure(
                e => e.Split('@').Length == 2,
                DomainErrors.InvalidFormat(Name))
            .Map(e => new Email(e));

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}