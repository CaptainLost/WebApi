using Microsoft.Extensions.DependencyInjection;

namespace Users.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersDomain(this IServiceCollection services)
    {
        return services;
    }
}
