using Microsoft.EntityFrameworkCore;
using Wms.Domain.Catalog;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class CatalogModelTests
{
    private static WmsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseNpgsql("Host=localhost;Database=wms_model_tests;Username=test;Password=test")
            .Options;

        return new WmsDbContext(options);
    }

    [Fact]
    public void CatalogEntities_UseCatalogSchema()
    {
        using var context = CreateContext();

        Assert.Equal("catalog", context.Model.FindEntityType(typeof(ProductCategory))!.GetSchema());
        Assert.Equal("catalog", context.Model.FindEntityType(typeof(Product))!.GetSchema());
        Assert.Equal("catalog", context.Model.FindEntityType(typeof(ProductVariant))!.GetSchema());
    }

    [Fact]
    public void ProductVariant_HasUniqueInternalCode()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(ProductVariant))!;
        var internalCodeIndex = entityType
            .GetIndexes()
            .Single(index => index.Properties
                .Select(property => property.Name)
                .SequenceEqual([nameof(ProductVariant.InternalCode)]));

        Assert.True(internalCodeIndex.IsUnique);
        Assert.Equal(
            "'PV-' || LPAD(nextval('catalog.product_internal_code_sequence')::text, 8, '0')",
            entityType.FindProperty(nameof(ProductVariant.InternalCode))!.GetDefaultValueSql());
    }

    [Fact]
    public void Product_HasUniqueNameInsideCategory()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(Product))!;
        var nameIndex = entityType
            .GetIndexes()
            .Single(index => index.Properties
                .Select(property => property.Name)
                .SequenceEqual([nameof(Product.CategoryId), nameof(Product.NormalizedName)]));

        Assert.True(nameIndex.IsUnique);
    }
}
