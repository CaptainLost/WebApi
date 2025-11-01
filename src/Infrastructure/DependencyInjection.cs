using Microsoft.Extensions.DependencyInjection;
using Application.Repositories.Users;
using Infrastructure.Repositories.Users;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();

        return services;
    }
}
