using Authentication.Application.Abstractions.Repositories;
using Authentication.Persistence.Repositories;
using Core.Domain.Entities;
using Core.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPermissionsRepository, PermissionsRepository>();

        services.AddIdentityCore<User>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.SignIn.RequireConfirmedAccount = false;
        })
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
