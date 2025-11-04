using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Extensions;
using Domain.Users;
using Infrastructure.Authentication.Services;
using Infrastructure.Users.DbContext;
using Infrastructure.Messaging.InMemoryCommandDispatcher;
using Infrastructure.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>();
        services.AddCommandHandlers();

        services.AddDbContext<UsersDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme);

        services.AddAuthorizationBuilder();

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
