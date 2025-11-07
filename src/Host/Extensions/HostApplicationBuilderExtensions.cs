using Application;
using Infrastructure;
using Presentation;

namespace Host.Extensions;

internal static class HostApplicationBuilderExtensions
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddOpenApiServices();
        builder.AddApplicationLayers();
        builder.AddCorsPolicy();

        return builder;
    }

    private static WebApplicationBuilder AddOpenApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        return builder;
    }

    private static WebApplicationBuilder AddApplicationLayers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Environment, builder.Configuration)
            .AddPresentation();

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
