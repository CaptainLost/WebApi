namespace Users.Domain.Configuration;

public sealed class DefaultUserSettings
{
    public const string SectionName = "DefaultUser";
    public const string ValidationFailureMessage = "Default user settings are invalid.";

    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public static bool Validate(DefaultUserSettings settings)
    {
        return
            !string.IsNullOrWhiteSpace(settings.Username) &&
            !string.IsNullOrWhiteSpace(settings.Email) &&
            !string.IsNullOrWhiteSpace(settings.Password);
    }
}
