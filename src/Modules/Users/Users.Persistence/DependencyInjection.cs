using Users.Domain.Entities;
using Core.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Abstractions.Repositories;
using Users.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Users.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Users.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database connection string not found");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
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
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
