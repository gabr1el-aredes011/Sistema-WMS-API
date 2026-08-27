using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Domain.Catalog;

namespace Wms.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "catalog");
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(product => product.NormalizedName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(product => product.Type)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(product => product.ItemType)
            .HasMaxLength(40)
            .IsRequired();
        builder.Property(product => product.Model).HasMaxLength(100);
        builder.Property(product => product.IsActive).IsRequired();
        builder.Property(product => product.CreatedAtUtc).IsRequired();

        builder.HasIndex(product => new
        {
            product.CategoryId,
            product.NormalizedName
        })
            .IsUnique()
            .HasDatabaseName("ux_products_category_normalized_name");

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
