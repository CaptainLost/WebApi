using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
