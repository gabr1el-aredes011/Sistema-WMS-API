using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Identity;

namespace Wms.Infrastructure.Persistence.Configurations.Identity;

public sealed class ApplicationRoleConfiguration
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(
        EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("roles", "auth");

        builder
            .Property(role => role.Description)
            .HasMaxLength(300);

        builder
            .Property(role => role.IsSystem)
            .IsRequired();

        builder
            .Property(role => role.CreatedAtUtc)
            .IsRequired();
    }
}