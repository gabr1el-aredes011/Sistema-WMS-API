using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Shipping;

namespace Wms.Infrastructure.Persistence.Configurations.Shipping;

public sealed class PickupRequestConfiguration : IEntityTypeConfiguration<PickupRequest>
{
    public void Configure(EntityTypeBuilder<PickupRequest> builder)
    {
        builder.ToTable("pickup_requests", "shipping");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Code)
            .HasMaxLength(20)
            .HasDefaultValueSql("'COL-' || LPAD(nextval('shipping.pickup_code_sequence')::text, 8, '0')")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(item => item.OrderReference).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(500);
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(item => item.Code).IsUnique().HasDatabaseName("ux_pickup_requests_code");
        builder.HasIndex(item => item.PublicAccessToken).IsUnique().HasDatabaseName("ux_pickup_requests_public_token");
        builder.HasIndex(item => new { item.Status, item.ScheduledAtUtc }).HasDatabaseName("ix_pickup_requests_status_schedule");
        builder.HasOne(item => item.Carrier).WithMany(item => item.PickupRequests).HasForeignKey(item => item.CarrierId).OnDelete(DeleteBehavior.Restrict);
    }
}
