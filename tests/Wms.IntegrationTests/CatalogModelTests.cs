using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
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
        Assert.Equal("catalog", context.Model.FindEntityType(typeof(UnitOfMeasure))!.GetSchema());
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

    [Fact]
    public void ProductLifecycle_HasSoftDeleteFilters()
    {
        using var context = CreateContext();

        Assert.NotEmpty(context.Model.FindEntityType(typeof(Product))!.GetDeclaredQueryFilters());
        Assert.NotEmpty(context.Model.FindEntityType(typeof(ProductVariant))!.GetDeclaredQueryFilters());
    }

    [Fact]
    public void ProductColor_HasCorporateCatalogSeed()
    {
        using var context = CreateContext();

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entityType = designTimeModel.FindEntityType(typeof(ProductColor))!;
        var colors = entityType.GetSeedData()
            .Select(item => item[nameof(ProductColor.Name)])
            .ToArray();

        Assert.Equal(4, colors.Length);
        Assert.Contains("Cinza", colors);
        Assert.Contains("Laranja", colors);
        Assert.Contains("Preto", colors);
        Assert.Contains("Azul", colors);
    }

    [Fact]
    public void UnitOfMeasure_HasInitialOperationalCatalogSeed()
    {
        using var context = CreateContext();

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entityType = designTimeModel.FindEntityType(typeof(UnitOfMeasure))!;
        var units = entityType.GetSeedData()
            .Select(item => item[nameof(UnitOfMeasure.Code)])
            .ToArray();

        Assert.Equal(5, units.Length);
        Assert.Contains("UN", units);
        Assert.Contains("PAR", units);
        Assert.Contains("KIT", units);
        Assert.Contains("KG", units);
        Assert.Contains("M", units);
    }
}
