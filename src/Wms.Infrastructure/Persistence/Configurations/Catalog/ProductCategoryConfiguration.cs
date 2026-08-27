using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;

namespace Wms.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductCategoryConfiguration
    : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("product_categories", "catalog");
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(category => category.NormalizedName)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.CreatedAtUtc).IsRequired();

        builder.HasIndex(category => category.NormalizedName)
            .IsUnique()
            .HasDatabaseName("ux_product_categories_normalized_name");
    }
}
