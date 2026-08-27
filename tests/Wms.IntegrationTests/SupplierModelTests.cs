using Microsoft.EntityFrameworkCore;
using Wms.Domain.Suppliers;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class SupplierModelTests
{
    [Fact]
    public void Supplier_UsesDedicatedSchemaAndUniqueTaxId()
    {
        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseNpgsql("Host=localhost;Database=wms_model_tests;Username=test;Password=test")
            .Options;
        using var context = new WmsDbContext(options);

        var entityType = context.Model.FindEntityType(typeof(Supplier))!;
        var taxIdIndex = entityType
            .GetIndexes()
            .Single(index => index.Properties
                .Select(property => property.Name)
                .SequenceEqual([nameof(Supplier.TaxId)]));

        Assert.Equal("suppliers", entityType.GetSchema());
        Assert.Equal("suppliers", entityType.GetTableName());
        Assert.True(taxIdIndex.IsUnique);
    }
}
