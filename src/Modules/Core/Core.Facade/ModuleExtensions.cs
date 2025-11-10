using Core.Application;
using Core.Domain;
using Core.Infrastructure;
using Core.Persistence;
using Core.Presentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Facade;

public static class ModuleExtensions
{
    public static IServiceCollection AddCoreModule(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services
            .AddCoreDomain()
            .AddCoreApplication()
            .AddCorePersistence(configuration)
            .AddCoreInfrastructure(environment, configuration)
            .AddCorePresentation();

        return services;
    }

    public static WebApplication ConfigureCoreModule(this WebApplication app)
    {
        app.ConfigureCorePresentation();

        return app;
    }
}
