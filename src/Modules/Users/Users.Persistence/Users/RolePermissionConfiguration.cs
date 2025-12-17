using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;
using Users.Persistence.Constants;

namespace Users.Persistence.Users;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(TableNames.RolePermissions);

        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            Create(Role.Registered, Permission.GetUser),
            Create(Role.Registered, Permission.GetUserList),

            Create(Role.Administrator, Permission.GetUser),
            Create(Role.Administrator, Permission.ModifyUser),
            Create(Role.Administrator, Permission.CreateUser),
            Create(Role.Administrator, Permission.DeleteUser),
            Create(Role.Administrator, Permission.AssignRole),
            Create(Role.Administrator, Permission.GetUserList),
            Create(Role.Administrator, Permission.BanUser),
            Create(Role.Administrator, Permission.UnbanUser),
            Create(Role.Administrator, Permission.UnbanAllBans),
            Create(Role.Administrator, Permission.UnbanSingleBan));
    }

    private static RolePermission Create(Role role, Permission permission)
    {
        return new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        };
    }
}
