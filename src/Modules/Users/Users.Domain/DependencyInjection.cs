using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Configuration;

namespace Users.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersDomain(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AdminUserSettings>(configuration.GetSection(AdminUserSettings.SectionName));
        services.AddOptions<AdminUserSettings>()
            .Validate(AdminUserSettings.Validate, AdminUserSettings.ValidationFailureMessage)
            .ValidateOnStart();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddOptions<JwtSettings>()
            .Validate(JwtSettings.Validate, JwtSettings.ValidationFailureMessage)
            .ValidateOnStart();

        services.Configure<UserSettings>(configuration.GetSection(UserSettings.SectionName));
        services.AddOptions<UserSettings>()
            .Validate(UserSettings.Validate, UserSettings.ValidationFailureMessage)
            .ValidateOnStart();

        return services;
    }
}
