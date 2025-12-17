using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Persistence.Constants;

namespace Users.Persistence.Users;

internal sealed class UserBanConfiguration : IEntityTypeConfiguration<UserBan>
{
    public void Configure(EntityTypeBuilder<UserBan> builder)
    {
        builder.ToTable(TableNames.UserBans);

        builder.HasKey(ub => ub.Id);

        builder.Property(ub => ub.Id)
            .ValueGeneratedNever();

        builder.Property(ub => ub.UserId)
            .IsRequired();

        builder.Property(ub => ub.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ub => ub.BannedBy)
            .IsRequired();

        builder.Property(ub => ub.BannedAt)
            .IsRequired();

        builder.Property(ub => ub.ExpiresAt)
            .IsRequired();

        builder.Property(ub => ub.UnbannedAt);

        builder.Property(ub => ub.UnbannedBy);

        builder.HasIndex(ub => ub.UserId);
    }
}
