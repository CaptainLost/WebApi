using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.Application;
using Users.Domain;
using Users.Infrastructure;
using Users.Persistence;
using Users.Presentation;

namespace Users.Facade;

public static class ModuleExtensions
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services
            .AddUsersDomain()
            .AddUsersApplication()
            .AddUsersPersistence(configuration)
            .AddUsersInfrastructure(environment, configuration)
            .AddUsersPresentation();

        return services;
    }

    public static WebApplication ConfigureUsersModule(this WebApplication app)
    {
        app.ConfigureUsersPresentation();

        return app;
    }
}
