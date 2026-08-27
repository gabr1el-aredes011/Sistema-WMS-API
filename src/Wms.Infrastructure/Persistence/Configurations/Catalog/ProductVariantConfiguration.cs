using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;

namespace Wms.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductVariantConfiguration
    : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants", "catalog");
        builder.HasKey(variant => variant.Id);

        builder.Property(variant => variant.InternalCode)
            .HasMaxLength(32)
            .HasDefaultValueSql(
                "'PV-' || LPAD(nextval('catalog.product_internal_code_sequence')::text, 8, '0')")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(variant => variant.Color)
            .HasMaxLength(60)
            .IsRequired();
        builder.Property(variant => variant.ExternalReference).HasMaxLength(64);
        builder.Property(variant => variant.ExternalBarcode).HasMaxLength(64);
        builder.Property(variant => variant.UnitOfMeasure)
            .HasMaxLength(12)
            .IsRequired();
        builder.Property(variant => variant.IsActive).IsRequired();
        builder.Property(variant => variant.CreatedAtUtc).IsRequired();

        builder.HasIndex(variant => variant.InternalCode)
            .IsUnique()
            .HasDatabaseName("ux_product_variants_internal_code");
        builder.HasIndex(variant => variant.ExternalReference)
            .HasDatabaseName("ix_product_variants_external_reference");
        builder.HasIndex(variant => variant.ExternalBarcode)
            .IsUnique()
            .HasFilter("\"ExternalBarcode\" IS NOT NULL")
            .HasDatabaseName("ux_product_variants_external_barcode");

        builder.HasOne(variant => variant.Product)
            .WithMany(product => product.Variants)
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
