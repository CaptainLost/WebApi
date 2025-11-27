using Authentication.Persistence.Constants;
using Users.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Persistence.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(TableNames.RolePermission);

        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.HasData(
            Create(Role.User, PermissionType.ReadUser),
            Create(Role.User, PermissionType.AccessUsers),
            Create(Role.Admin, PermissionType.FullAccess));
    }

    private static RolePermission Create(Role role, PermissionType permission)
    {
        return new RolePermission
        {
            RoleId = role.Id,
            PermissionId = (int)permission
        };
    }
}