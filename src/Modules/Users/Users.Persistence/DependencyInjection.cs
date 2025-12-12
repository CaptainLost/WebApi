using Core.Persistence.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Users;
using Users.Persistence.Database;
using Users.Persistence.Users;

namespace Users.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection string not found");

        services.AddDbContext<UsersDbContext>((serviceProvider, options) =>
        {
            DomainEventDispatcherInterceptor interceptor = serviceProvider
                .GetRequiredService<DomainEventDispatcherInterceptor>();

            options.UseSqlite(connectionString)
                .AddInterceptors(interceptor);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
