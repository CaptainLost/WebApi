using Application;
using Authentication.Facade;
using Infrastructure;
using Persistence;
using Presentation;

namespace Host.Extensions;

internal static class HostApplicationBuilderExtensions
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddOpenApiServices();
        builder.AddApplicationModules();
        builder.AddCorsPolicy();

        return builder;
    }

    private static WebApplicationBuilder AddOpenApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        return builder;
    }

    private static WebApplicationBuilder AddApplicationModules(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApplication()
            .AddPersistence(builder.Configuration)
            .AddInfrastructure(builder.Environment, builder.Configuration)
            .AddPresentation()
            .AddAuthenticationModule(builder.Environment, builder.Configuration);

        return builder;
    }

    private static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                IConfigurationSection corsSection = builder.Configuration.GetSection("Cors");
                string[] allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? [];

                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });

        return builder;
    }
}
