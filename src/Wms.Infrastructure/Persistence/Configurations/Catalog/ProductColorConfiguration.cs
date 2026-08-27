using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;

namespace Wms.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductColorConfiguration
    : IEntityTypeConfiguration<ProductColor>
{
    private static readonly DateTimeOffset SeededAtUtc =
        new(2026, 8, 27, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.ToTable("product_colors", "catalog");
        builder.HasKey(color => color.Id);

        builder.Property(color => color.Name).HasMaxLength(60).IsRequired();
        builder.Property(color => color.NormalizedName).HasMaxLength(60).IsRequired();
        builder.Property(color => color.HexCode).HasMaxLength(7).IsRequired();
        builder.Property(color => color.IsActive).IsRequired();
        builder.Property(color => color.CreatedAtUtc).IsRequired();

        builder.HasIndex(color => color.NormalizedName)
            .IsUnique()
            .HasDatabaseName("ux_product_colors_normalized_name");

        builder.HasData(
            Create("30000000-0000-0000-0000-000000000001", "Cinza", "#808080"),
            Create("30000000-0000-0000-0000-000000000002", "Laranja", "#F97316"),
            Create("30000000-0000-0000-0000-000000000003", "Preto", "#111827"),
            Create("30000000-0000-0000-0000-000000000004", "Azul", "#2563EB"));
    }

    private static ProductColor Create(string id, string name, string hexCode) =>
        new()
        {
            Id = Guid.Parse(id),
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            HexCode = hexCode,
            IsActive = true,
            CreatedAtUtc = SeededAtUtc
        };
}
