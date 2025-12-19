using System.Threading.RateLimiting;
using Core.Presentation.RateLimiting;
using Host.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

namespace Host.Extensions;

internal static class HostApplicationBuilderExtensions
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddExceptionHandlingServices();
        builder.AddOpenApiServices();
        builder.AddRateLimiter();
        builder.AddApplicationModules();
        builder.AddCorsPolicy();

        return builder;
    }

    private static WebApplicationBuilder AddExceptionHandlingServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        return builder;
    }

    private static WebApplicationBuilder AddOpenApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
                };

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                return Task.CompletedTask;
            });
        });

        return builder;
    }

    private static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.AddFixedWindowLimiter(RateLimiterNames.AuthFixed, options =>
            {
                options.Window = TimeSpan.FromMinutes(1);
                options.PermitLimit = 5;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            });

            rateLimiterOptions.AddFixedWindowLimiter(RateLimiterNames.WriteFixed, options =>
            {
                options.Window = TimeSpan.FromMinutes(1);
                options.PermitLimit = 20;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
            });

            rateLimiterOptions.AddFixedWindowLimiter(RateLimiterNames.ReadFixed, options =>
            {
                options.Window = TimeSpan.FromMinutes(1);
                options.PermitLimit = 60;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 10;
            });

            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return builder;
    }

    private static WebApplicationBuilder AddApplicationModules(this WebApplicationBuilder builder)
    {
        ModuleRegistry.RegisterModules(builder);

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
