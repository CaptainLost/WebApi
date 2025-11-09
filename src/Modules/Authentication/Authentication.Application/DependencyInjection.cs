using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationApplication(this IServiceCollection services)
    {
        return services;
    }
}
