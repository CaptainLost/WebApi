using System.Security.Cryptography;

namespace Users.Domain.ValueObjects;

public static class PasswordHashingConstants
{
    /// <summary>
    /// Size of the derived key in bytes (64 bytes = 512 bits).
    /// </summary>
    public const int KeySize = 64;

    /// <summary>
    /// Size of the salt in bytes (64 bytes = 512 bits).
    /// </summary>
    public const int SaltSize = 64;

    /// <summary>
    /// Number of iterations for PBKDF2 algorithm.
    /// </summary>
    public const int Iterations = 350000;

    /// <summary>
    /// Hash algorithm used for PBKDF2 (SHA512).
    /// </summary>
    public static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /// <summary>
    /// Expected length of the hash string in hexadecimal format (64 bytes = 128 hex characters).
    /// </summary>
    public const int HashHexLength = KeySize * 2;
}
