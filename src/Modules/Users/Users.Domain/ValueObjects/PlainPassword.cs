using System.Text.RegularExpressions;
using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.ValueObjects;

public sealed partial class PlainPassword : ValueObject
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    private PlainPassword(string value)
    {
        Value = value;
    }

    private PlainPassword()
    {
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<PlainPassword> Create(string password)
    {
        if (password is null)
        {
            return Result.Failure<PlainPassword>(PlainPasswordErrors.Empty);
        }

        return Result.Create(password)
            .Ensure(
                p => !string.IsNullOrWhiteSpace(p),
                PlainPasswordErrors.Empty)
            .Ensure(
                p => p.Length >= MinLength,
                PlainPasswordErrors.TooShort)
            .Ensure(
                p => p.Length <= MaxLength,
                PlainPasswordErrors.TooLong)
            .Ensure(
                p => UppercaseRegex().IsMatch(p),
                PlainPasswordErrors.MissingUppercase)
            .Ensure(
                p => LowercaseRegex().IsMatch(p),
                PlainPasswordErrors.MissingLowercase)
            .Ensure(
                p => DigitRegex().IsMatch(p),
                PlainPasswordErrors.MissingDigit)
            .Ensure(
                p => SpecialCharacterRegex().IsMatch(p),
                PlainPasswordErrors.MissingSpecialCharacter)
            .Map(p => new PlainPassword(p));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowercaseRegex();

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex SpecialCharacterRegex();
}
