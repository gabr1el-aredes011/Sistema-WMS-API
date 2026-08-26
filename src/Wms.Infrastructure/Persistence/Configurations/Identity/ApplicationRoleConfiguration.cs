using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Identity;
using Wms.Infrastructure.Persistence.Seed;

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

        builder.HasData(AuthorizationSeedData.Roles.Select(seed => new
        {
            seed.Id,
            Name = seed.Definition.Name,
            NormalizedName = seed.Definition.Name.ToUpperInvariant(),
            seed.Definition.Description,
            IsSystem = true,
            CreatedAtUtc = AuthorizationSeedData.SeededAtUtc,
            ConcurrencyStamp = $"system-role-{seed.Id:N}"
        }));
    }
}
