using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.Application.Abstractions.Services;
using Users.Infrastructure.Authorization;
using Users.Infrastructure.Services;

namespace Users.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.AddAuthenticationServices(environment);

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(
    this IServiceCollection services,
    IHostEnvironment environment)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(36);
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

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
