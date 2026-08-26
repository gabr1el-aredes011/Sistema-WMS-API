using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Identity;

namespace Wms.Infrastructure.Persistence.Configurations.Identity;

public sealed class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(
        EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users", "auth");

        builder
            .Property(user => user.FullName)
            .HasMaxLength(160)
            .IsRequired();

        builder
            .Property(user => user.IsActive)
            .IsRequired();

        builder
            .Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder
            .HasIndex(user => user.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("ux_users_normalized_email");
    }
}