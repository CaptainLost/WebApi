namespace Users.Domain.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";
    public const string ValidationFailureMessage = "JWT settings are invalid.";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; }

    public static bool Validate(JwtSettings settings)
    {
        return
            !string.IsNullOrWhiteSpace(settings.Secret) &&
            !string.IsNullOrWhiteSpace(settings.Issuer) &&
            !string.IsNullOrWhiteSpace(settings.Audience) &&
            settings.ExpirationInMinutes > 0;
    }
}
