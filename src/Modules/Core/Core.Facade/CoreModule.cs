using Core.Application;
using Core.Domain;
using Core.Facade.Abstractions;
using Core.Infrastructure;
using Core.Persistence;
using Core.Presentation;
using Microsoft.AspNetCore.Builder;

namespace Core.Facade;

public sealed class CoreModule : IModule
{
    public string Name => "Core";
    public int Order => int.MinValue;

    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddCoreDomain()
            .AddCoreApplication()
            .AddCorePersistence(builder.Configuration)
            .AddCoreInfrastructure(builder.Environment, builder.Configuration)
            .AddCorePresentation();
    }

    public void ConfigureApplication(WebApplication app)
    {
        app.ConfigureCorePresentation();
    }
}
