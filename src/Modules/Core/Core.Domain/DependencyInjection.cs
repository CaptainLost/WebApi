using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDomain(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
