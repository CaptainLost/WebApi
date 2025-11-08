using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.HasData(
            Create(Role.User, PermissionType.ReadUser),
            Create(Role.User, PermissionType.AccessUsers),
            Create(Role.Admin, PermissionType.ReadUser),
            Create(Role.Admin, PermissionType.AccessUsers));
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