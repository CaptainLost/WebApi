using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Users;
using Infrastructure.Authentication.Services;
using Infrastructure.Users.DbContext;
using Infrastructure.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
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
