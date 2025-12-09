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
        services.Configure<DefaultUserSettings>(configuration.GetSection(DefaultUserSettings.SectionName));
        services.AddOptions<DefaultUserSettings>()
            .Validate(DefaultUserSettings.Validate, DefaultUserSettings.ValidationFailureMessage)
            .ValidateOnStart();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddOptions<JwtSettings>()
            .Validate(JwtSettings.Validate, JwtSettings.ValidationFailureMessage)
            .ValidateOnStart();

        return services;
    }
}
