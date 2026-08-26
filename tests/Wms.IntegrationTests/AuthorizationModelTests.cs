using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Wms.Application.Authorization;
using Wms.Infrastructure.Authorization;
using Wms.Infrastructure.Identity;
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

    [Fact]
    public void AuthorizationCatalog_IsIncludedInTheEfModel()
    {
        using var context = CreateContext();

        var designTimeModel = context
            .GetService<IDesignTimeModel>()
            .Model;

        var permissionSeed = designTimeModel
            .FindEntityType(typeof(Permission))!
            .GetSeedData();
        var roleSeed = designTimeModel
            .FindEntityType(typeof(ApplicationRole))!
            .GetSeedData();
        var rolePermissionSeed = designTimeModel
            .FindEntityType(typeof(RolePermission))!
            .GetSeedData();

        Assert.Equal(SystemPermissions.All.Count, permissionSeed.Count());
        Assert.Equal(SystemRoles.All.Count, roleSeed.Count());
        Assert.Equal(
            SystemRoles.All.Sum(role => role.PermissionCodes.Count),
            rolePermissionSeed.Count());
    }
}
