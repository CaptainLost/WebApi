namespace Users.Domain.Configuration;

public sealed class UserSettings
{
    public const string SectionName = "UserSettings";
    public const string ValidationFailureMessage = "User settings are invalid.";

    public int MaxFailedLoginAttempts { get; init; }
    public int BaseLockoutDurationMinutes { get; init; }

    public static bool Validate(UserSettings settings)
    {
        return
            settings.MaxFailedLoginAttempts > 0 &&
            settings.BaseLockoutDurationMinutes > 0;
    }
}
