using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;
using Wms.Domain.Operations;

namespace Wms.Infrastructure.Persistence.Configurations.Operations;

public sealed class OperationsConfiguration : IEntityTypeConfiguration<SalesOrder>, IEntityTypeConfiguration<OrderLine>,
    IEntityTypeConfiguration<StockBalance>, IEntityTypeConfiguration<StockMovement>, IEntityTypeConfiguration<OrderEvent>, IEntityTypeConfiguration<OrderDocument>
{
    public void Configure(EntityTypeBuilder<SalesOrder> b)
    {
        b.ToTable("orders", "operations"); b.HasKey(x => x.Id);
        b.Property(x => x.InvoiceKey).HasMaxLength(44); b.HasIndex(x => x.InvoiceKey).IsUnique();
        b.Property(x => x.InvoiceNumber).HasMaxLength(9);
        b.Property(x => x.Customer).HasMaxLength(200); b.Property(x => x.Channel).HasMaxLength(30);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Events).WithOne().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<OrderLine> b)
    {
        b.ToTable("order_lines", "operations", t => t.HasCheckConstraint("ck_line_quantities", "\"Quantity\" > 0 AND \"Reserved\" >= 0 AND \"Reserved\" <= \"Quantity\""));
        b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 3); b.Property(x => x.Reserved).HasPrecision(18, 3);
        b.HasOne<ProductVariant>().WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<StockBalance> b)
    {
        b.ToTable("stock_balances", "operations", t => t.HasCheckConstraint("ck_stock_quantities", "\"OnHand\" >= 0 AND \"Reserved\" >= 0 AND \"Reserved\" <= \"OnHand\""));
        b.HasKey(x => x.VariantId); b.Property(x => x.OnHand).HasPrecision(18, 3); b.Property(x => x.Reserved).HasPrecision(18, 3);
        b.HasOne<ProductVariant>().WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<StockMovement> b)
    {
        b.ToTable("stock_movements", "operations"); b.HasKey(x => x.Id);
        b.Property(x => x.Quantity).HasPrecision(18, 3);
        b.HasIndex(x => new { x.OperationId, x.VariantId }).IsUnique();
        b.HasOne<ProductVariant>().WithMany().HasForeignKey(x => x.VariantId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<OrderEvent> b)
    {
        b.ToTable("order_events", "operations"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
    }
    public void Configure(EntityTypeBuilder<OrderDocument> b)
    {
        b.ToTable("order_documents", "operations"); b.HasKey(x => x.Id);
        b.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.SalesOrderId, x.Kind }).IsUnique();
    }
}
