using Microsoft.EntityFrameworkCore;
using Wms.Infrastructure.Authorization;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class AuthorizationModelTests
{
    private static WmsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=wms_model_tests;Username=test;Password=test")
            .Options;

        return new WmsDbContext(options);
    }

    [Fact]
    public void Permission_UsesAuthSchemaAndUniqueCode()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(Permission));

        Assert.NotNull(entityType);
        Assert.Equal("permissions", entityType.GetTableName());
        Assert.Equal("auth", entityType.GetSchema());

        var codeIndex = entityType
            .GetIndexes()
            .Single(index => index.Properties
                .Select(property => property.Name)
                .SequenceEqual([nameof(Permission.Code)]));

        Assert.True(codeIndex.IsUnique);
    }

    [Fact]
    public void RolePermission_UsesCompositePrimaryKey()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(RolePermission));
        var primaryKey = entityType?.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Equal(
            [nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId)],
            primaryKey.Properties.Select(property => property.Name));
    }
}
