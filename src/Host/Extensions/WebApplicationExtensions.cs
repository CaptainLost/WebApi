namespace Host.Extensions;

internal static class WebApplicationExtensions
{
    internal static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
    {
        await app.ConfigureDevelopmentFeatures();
        app.ConfigureMiddleware();
        app.ConfigureModules();

        return app;
    }

    private static async Task<WebApplication> ConfigureDevelopmentFeatures(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            await app.ApplyMigrationsAsync();

            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "API V1");
            });
        }

        return app;
    }

    private static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // app.UseHttpsRedirection();
        app.UseCors();
        app.UseRateLimiter();
        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static WebApplication ConfigureModules(this WebApplication app)
    {
        ModuleRegistry.ConfigureModules(app);

        return app;
    }
}
