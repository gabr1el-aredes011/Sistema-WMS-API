using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Shipping;

namespace Wms.Infrastructure.Persistence.Configurations.Shipping;

public sealed class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
{
    public void Configure(EntityTypeBuilder<Carrier> builder)
    {
        builder.ToTable("carriers", "shipping");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).HasMaxLength(180).IsRequired();
        builder.Property(item => item.NormalizedName).HasMaxLength(180).IsRequired();
        builder.Property(item => item.TaxId).HasMaxLength(14);
        builder.Property(item => item.ContactName).HasMaxLength(160);
        builder.Property(item => item.Email).HasMaxLength(256);
        builder.Property(item => item.Phone).HasMaxLength(30);
        builder.HasIndex(item => item.NormalizedName).IsUnique().HasDatabaseName("ux_carriers_normalized_name");
        builder.HasIndex(item => item.TaxId).IsUnique().HasFilter("\"TaxId\" IS NOT NULL").HasDatabaseName("ux_carriers_tax_id");
    }
}
