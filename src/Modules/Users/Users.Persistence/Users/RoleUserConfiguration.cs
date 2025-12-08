using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Persistence.Constants;

namespace Users.Persistence.Users;

internal sealed class RoleUserConfiguration : IEntityTypeConfiguration<RoleUser>
{
    public void Configure(EntityTypeBuilder<RoleUser> builder)
    {
        builder.ToTable(TableNames.RoleUser);

        builder.HasKey(ru => new { ru.RoleId, ru.UserId });

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ru => ru.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ru => ru.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
