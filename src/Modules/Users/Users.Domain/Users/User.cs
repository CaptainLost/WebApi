using Core.Domain.Messaging;
using Core.Domain.Primitives;
using Users.Domain.Configuration;
using Users.Domain.ValueObjects;

namespace Users.Domain.Users;

public sealed class User : Entity
{
    public Username Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; set; } = string.Empty;
    public byte[] PasswordSalt { get; set; } = [];
    public Nickname Nickname { get; private set; }
    public DateTime CreationDate { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLockout { get; set; }
    public int LockoutCount { get; set; }

    public ICollection<Role> Roles { get; set; } = [];

    private User(Guid id, Username username, Email email, string passwordHash, byte[] passwordSalt, Nickname nickname)
        : base(id)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Nickname = nickname;
        CreationDate = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutEnd = null;
        LastLockout = null;
        LockoutCount = 0;
    }

    public static Result<User> Create(Guid id, Username username, Email email, string passwordHash, byte[] passwordSalt, Nickname nickname)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(UserErrors.InvalidPasswordHash);
        }

        if (passwordSalt == null || passwordSalt.Length == 0)
        {
            return Result.Failure<User>(UserErrors.InvalidPasswordHash);
        }

        User user = new User(id, username, email, passwordHash, passwordSalt, nickname);

        return Result.Success(user);
    }

    public bool HasRole(string roleName)
    {
        return Roles.Any(r => r.Name == roleName);
    }

    public Result AssignRole(Role role)
    {
        if (HasRole(role.Name))
        {
            return UserErrors.AlreadyHasRole(role.Name);
        }

        Roles.Add(role);

        return Result.Success();
    }

    public bool IsLockedOut()
    {
        return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    }

    public void RecordFailedLogin(UserSettings settings)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= settings.MaxFailedLoginAttempts)
        {
            LockoutCount++;
            LockoutEnd = DateTime.UtcNow.AddMinutes(CalculateLockoutDurationInMinutes(settings));
            LastLockout = DateTime.UtcNow;
        }
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
        // LockoutCount = 0;
    }

    public Result UpdatePassword(string newPasswordHash, byte[] newPasswordSalt)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            return Result.Failure(UserErrors.InvalidPasswordHash);
        }

        if (newPasswordSalt == null || newPasswordSalt.Length == 0)
        {
            return Result.Failure(UserErrors.InvalidPasswordHash);
        }

        PasswordHash = newPasswordHash;
        PasswordSalt = newPasswordSalt;

        return Result.Success();
    }

    public HashSet<string> GetPermissions()
    {
        return Roles
            .SelectMany(role => role.Permissions)
            .Select(permission => permission.Name)
            .ToHashSet();
    }

    private int CalculateLockoutDurationInMinutes(UserSettings settings)
    {
        return settings.BaseLockoutDurationMinutes * (int)Math.Pow(2, LockoutCount - 1);
    }
}
