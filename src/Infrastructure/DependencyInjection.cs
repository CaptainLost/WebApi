using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Application.Users.Repositories;
using Infrastructure.Users.Repositories;
using Application.Users.DbContext;
using Domain.Users;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<UsersDbContext>();

        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddAuthorization();
        services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddApiEndpoints();

        return services;
    }
}
