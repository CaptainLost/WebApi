using Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.DbContext;

internal sealed class UsersDbContext : IdentityDbContext<User>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("users");
    }
}