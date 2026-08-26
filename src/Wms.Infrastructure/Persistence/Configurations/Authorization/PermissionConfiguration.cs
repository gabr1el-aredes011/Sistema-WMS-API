using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Authorization;

namespace Wms.Infrastructure.Persistence.Configurations.Authorization;

public sealed class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions", "auth");

        builder.HasKey(permission => permission.Id);

        builder
            .Property(permission => permission.Code)
            .HasMaxLength(160)
            .IsRequired();

        builder
            .Property(permission => permission.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder
            .Property(permission => permission.Module)
            .HasMaxLength(80)
            .IsRequired();

        builder
            .Property(permission => permission.Description)
            .HasMaxLength(300);

        builder
            .Property(permission => permission.IsSystem)
            .IsRequired();

        builder
            .Property(permission => permission.IsActive)
            .IsRequired();

        builder
            .Property(permission => permission.CreatedAtUtc)
            .IsRequired();

        builder
            .HasIndex(permission => permission.Code)
            .IsUnique()
            .HasDatabaseName("ux_permissions_code");

        builder
            .HasIndex(permission => new
            {
                permission.Module,
                permission.IsActive
            })
            .HasDatabaseName("ix_permissions_module_active");
    }
}
