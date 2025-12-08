using Microsoft.EntityFrameworkCore;
using Users.Persistence.Database;

namespace Host.Extensions;

internal static class MigrationExtensions
{
    internal static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await ApplyMigrationsAsync<UsersDbContext>(scope);
        await SeedDataAsync(scope);
    }

    private static async Task ApplyMigrationsAsync<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedDataAsync(IServiceScope scope)
    {
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
    }
}
