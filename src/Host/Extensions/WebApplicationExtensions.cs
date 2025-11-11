using Authentication.Facade;
using Core.Facade;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Facade;
using Users.Persistence.Database;

namespace Host.Extensions;

internal static class WebApplicationExtensions
{
    internal static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        await app.ApplyMigrations();
        await app.SeedData();

        app.ConfigureDevelopmentFeatures();
        app.ConfigureMiddleware();
        app.ConfigureModules();

        return app;
    }

    private static async Task<WebApplication> ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        return app;
    }

    private static async Task<WebApplication> SeedData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        await IdentitySeeder.SeedAsync(services);

        return app;
    }

    private static WebApplication ConfigureDevelopmentFeatures(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
            });
        }

        return app;
    }

    private static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static WebApplication ConfigureModules(this WebApplication app)
    {
        app
            .ConfigureCoreModule()
            .ConfigureAuthenticationModule()
            .ConfigureUsersModule();

        return app;
    }
}
