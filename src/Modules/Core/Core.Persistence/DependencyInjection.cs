using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddCorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {


        return services;
    }
}
