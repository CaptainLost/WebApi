using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Persistence.Constants;

namespace Users.Persistence.Users;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableNames.Roles);

        builder.HasKey(role => role.Id);
        builder.Property(role => role.Name).HasMaxLength(50).IsRequired();

        builder.HasMany(x => x.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder.HasMany(x => x.Users)
            .WithMany(u => u.Roles)
            .UsingEntity<RoleUser>();

        builder.HasData(Role.GetValues());
    }
}
