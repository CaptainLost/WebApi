namespace Users.Application.Configuration;

public sealed class DefaultUserSettings
{
    public const string SectionName = "DefaultUser";

    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
