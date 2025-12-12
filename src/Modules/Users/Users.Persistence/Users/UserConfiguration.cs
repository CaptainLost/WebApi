using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Domain.ValueObjects;
using Users.Persistence.Constants;

namespace Users.Persistence.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(u => u.Id);

        builder.Property(x => x.Username)
            .HasConversion(x => x.Value, v => Username.Create(v).Value)
            .HasMaxLength(Username.MaxLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, v => Email.Create(v).Value)
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.ComplexProperty(u => u.Password, passwordBuilder =>
        {
            passwordBuilder.Property(p => p.Hash)
                .HasColumnName($"{nameof(User.Password)}{nameof(Password.Hash)}")
                .IsRequired()
                .HasMaxLength(PasswordHashingConstants.HashHexLength);

            passwordBuilder.Property(p => p.Salt)
                .HasColumnName($"{nameof(User.Password)}{nameof(Password.Salt)}")
                .IsRequired()
                .HasMaxLength(PasswordHashingConstants.SaltSize);
        });

        builder.Property(x => x.Nickname)
            .HasConversion(x => x.Value, v => Nickname.Create(v).Value)
            .HasMaxLength(Nickname.MaxLength)
            .IsRequired();

        builder.Property(u => u.CreationDate)
            .IsRequired();

        builder.Property(u => u.FailedLoginAttempts)
            .IsRequired();

        builder.Property(u => u.LockoutEnd);

        builder.Property(u => u.LastLockout);

        builder.Property(u => u.LockoutCount)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
