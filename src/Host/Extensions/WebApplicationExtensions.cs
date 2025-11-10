using Authentication.Facade;
using Core.Facade;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Users.Facade;

namespace Host.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.ApplyDatabaseMigrations();
        //app.SeedDatabase();
        app.ConfigureDevelopmentFeatures();
        app.ConfigureMiddleware();
        app.ConfigureModules();

        return app;
    }

    private static WebApplication ApplyDatabaseMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();

        return app;
    }

    // private static WebApplication SeedDatabase(this WebApplication app)
    // {
    //     using IServiceScope scope = app.Services.CreateScope();
    //     IDatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();

    //     seeder.SeedAsync().GetAwaiter().GetResult();

    //     return app;
    // }

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
