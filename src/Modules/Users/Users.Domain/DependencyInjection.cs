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
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        return services;
    }
}
