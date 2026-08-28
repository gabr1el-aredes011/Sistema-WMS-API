using Microsoft.EntityFrameworkCore;
using Wms.Domain.Shipping;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class ShippingModelTests
{
    private static WmsDbContext CreateContext() => new(new DbContextOptionsBuilder<WmsDbContext>().UseNpgsql("Host=localhost;Database=wms_model_tests;Username=test;Password=test").Options);

    [Fact]
    public void ShippingEntities_UseDedicatedSchemaAndStableIdentifiers()
    {
        using var context = CreateContext();
        var carrier = context.Model.FindEntityType(typeof(Carrier))!;
        var pickup = context.Model.FindEntityType(typeof(PickupRequest))!;
        Assert.Equal("shipping", carrier.GetSchema());
        Assert.Equal("shipping", pickup.GetSchema());
        Assert.True(pickup.GetIndexes().Single(index => index.Properties.Select(property => property.Name).SequenceEqual([nameof(PickupRequest.PublicAccessToken)])).IsUnique);
        Assert.Equal("'COL-' || LPAD(nextval('shipping.pickup_code_sequence')::text, 8, '0')", pickup.FindProperty(nameof(PickupRequest.Code))!.GetDefaultValueSql());
    }
}
