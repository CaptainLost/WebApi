using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.ValueObjects;

public sealed class Password : ValueObject
{
    public static readonly int MinHashLength = PasswordHashingConstants.HashHexLength;
    public static readonly int MinSaltLength = PasswordHashingConstants.SaltSize;

    private Password(string hash, byte[] salt)
    {
        Hash = hash;
        Salt = salt;
    }

    private Password()
    {
    }

    public string Hash { get; private set; } = string.Empty;
    public byte[] Salt { get; private set; } = [];

    public static Result<Password> Create(string hash, byte[] salt)
    {
        return Result.Create((hash, salt))
            .Ensure(
                hs => !string.IsNullOrWhiteSpace(hs.hash),
                PasswordErrors.EmptyHash)
            .Ensure(
                hs => hs.hash.Length >= MinHashLength,
                PasswordErrors.HashTooShort)
            .Ensure(
                hs => hs.salt != null && hs.salt.Length > 0,
                PasswordErrors.EmptySalt)
            .Ensure(
                hs => hs.salt.Length >= MinSaltLength,
                PasswordErrors.SaltTooShort)
            .Map(hs => new Password(hs.hash, hs.salt));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Hash;
        yield return Salt;
    }
}
