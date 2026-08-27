using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Suppliers;

namespace Wms.Infrastructure.Persistence.Configurations.Suppliers;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", "suppliers");
        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(supplier => supplier.NormalizedLegalName).HasMaxLength(200).IsRequired();
        builder.Property(supplier => supplier.TradeName).HasMaxLength(200);
        builder.Property(supplier => supplier.TaxId).HasMaxLength(14).IsRequired();
        builder.Property(supplier => supplier.Email).HasMaxLength(256);
        builder.Property(supplier => supplier.Phone).HasMaxLength(30);
        builder.Property(supplier => supplier.IsActive).IsRequired();
        builder.Property(supplier => supplier.CreatedAtUtc).IsRequired();

        builder.HasIndex(supplier => supplier.TaxId)
            .IsUnique()
            .HasDatabaseName("ux_suppliers_tax_id");
        builder.HasIndex(supplier => supplier.NormalizedLegalName)
            .HasDatabaseName("ix_suppliers_normalized_legal_name");
    }
}
