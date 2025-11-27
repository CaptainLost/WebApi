using Core.Facade;
using Users.Facade;

namespace Host.Extensions;

internal static class WebApplicationExtensions
{
    internal static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        app.ConfigureDevelopmentFeatures();
        app.ConfigureMiddleware();
        app.ConfigureModules();

        return app;
    }

    private static WebApplication ConfigureDevelopmentFeatures(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();

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
            .ConfigureUsersModule();

        return app;
    }
}
