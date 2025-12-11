using System.Security.Cryptography;
using System.Text;
using Users.Application.Abstractions;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Authentication;

internal sealed class PasswordHashingService : IPasswordHashingService
{
    private const int _keySize = PasswordHashingConstants.KeySize;
    private const int _interations = PasswordHashingConstants.Iterations;
    private readonly static HashAlgorithmName _hashAlgorithm = PasswordHashingConstants.HashAlgorithm;

    public string HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(_keySize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            _interations,
            _hashAlgorithm,
            _keySize);

        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string passwordHash, byte[] salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _interations,
            _hashAlgorithm,
            _keySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(passwordHash));
    }
}
