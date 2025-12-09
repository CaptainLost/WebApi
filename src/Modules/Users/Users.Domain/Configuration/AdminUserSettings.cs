namespace Users.Domain.Configuration;

public sealed class AdminUserSettings
{
    public const string SectionName = "AdminUser";
    public const string ValidationFailureMessage = "Admin user settings are invalid.";

    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public static bool Validate(AdminUserSettings settings)
    {
        return
            !string.IsNullOrWhiteSpace(settings.Username) &&
            !string.IsNullOrWhiteSpace(settings.Email) &&
            !string.IsNullOrWhiteSpace(settings.Password);
    }
}
