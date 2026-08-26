using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Authorization;

namespace Wms.Infrastructure.Persistence.Configurations.Authorization;

public sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions", "auth");

        builder.HasKey(rolePermission => new
        {
            rolePermission.RoleId,
            rolePermission.PermissionId
        });

        builder
            .Property(rolePermission => rolePermission.AssignedAtUtc)
            .IsRequired();

        builder
            .HasOne(rolePermission => rolePermission.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(rolePermission => rolePermission.Permission)
            .WithMany(permission => permission.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(rolePermission => rolePermission.AssignedByUser)
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.AssignedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasIndex(rolePermission => rolePermission.PermissionId)
            .HasDatabaseName("ix_role_permissions_permission_id");

        builder
            .HasIndex(rolePermission => rolePermission.AssignedByUserId)
            .HasDatabaseName("ix_role_permissions_assigned_by_user_id");
    }
}
