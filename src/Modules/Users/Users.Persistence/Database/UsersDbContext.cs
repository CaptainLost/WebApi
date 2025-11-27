using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Persistence.Database;

public sealed class UsersDbContext : IdentityDbContext<User>
{
    internal DbSet<User> Users => Set<User>();
    internal DbSet<Role> Roles => Set<Role>();

    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
