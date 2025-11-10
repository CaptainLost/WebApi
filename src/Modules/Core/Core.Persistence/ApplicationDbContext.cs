using System.Reflection;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        Assembly[] assembliesToScan = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.FullName?.Contains("Persistence") == true)
            .ToArray();

        foreach (Assembly assembly in assembliesToScan)
        {
            builder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
