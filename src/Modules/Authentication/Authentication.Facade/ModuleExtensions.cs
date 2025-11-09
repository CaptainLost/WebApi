using Authentication.Application;
using Authentication.Domain;
using Authentication.Infrastructure;
using Authentication.Persistence;
using Authentication.Presentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Authentication.Facade;

public static class ModuleExtensions
{
    public static IServiceCollection AddAuthenticationModule(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services
            .AddAuthenticationDomain()
            .AddAuthenticationApplication()
            .AddAuthenticationPersistence(configuration)
            .AddAuthenticationInfrastructure(environment, configuration)
            .AddAuthenticationPresentation();

        return services;
    }
}
