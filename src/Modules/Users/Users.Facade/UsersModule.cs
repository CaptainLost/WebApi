using Core.Facade.Abstractions;
using Microsoft.AspNetCore.Builder;
using Users.Application;
using Users.Domain;
using Users.Infrastructure;
using Users.Persistence;
using Users.Presentation;

namespace Users.Facade;

public sealed class UsersModule : IModule
{
    public string Name => "Users";
    public int Order => 10;

    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddUsersDomain()
            .AddUsersApplication()
            .AddUsersPersistence(builder.Configuration)
            .AddUsersInfrastructure(builder.Environment, builder.Configuration)
            .AddUsersPresentation();
    }

    public void ConfigureApplication(WebApplication app)
    {
        app.ConfigureUsersPresentation();
    }
}
