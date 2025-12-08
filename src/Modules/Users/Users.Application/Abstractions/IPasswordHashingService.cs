namespace Users.Application.Abstractions;

public interface IPasswordHashingService
{
    string HashPassword(string password, out byte[] salt);
    bool VerifyPassword(string password, string passwordHash, byte[] salt);
}
