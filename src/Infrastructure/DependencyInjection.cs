using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Users;
using Infrastructure.Authentication.Services;
using Infrastructure.Users.DbContext;
using Infrastructure.Users.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddDbContext<UsersDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;

                if (environment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                }
                else
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                }
            });

        services.AddAuthorizationBuilder();

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
        })
            .AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
