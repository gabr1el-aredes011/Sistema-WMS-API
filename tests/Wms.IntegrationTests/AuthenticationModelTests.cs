using Microsoft.EntityFrameworkCore;
using Wms.Infrastructure.Authentication;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class AuthenticationModelTests
{
    [Fact]
    public void RefreshSession_UsesAuthSchemaAndUniqueTokenHash()
    {
        var options = new DbContextOptionsBuilder<WmsDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=wms_model_tests;Username=test;Password=test")
            .Options;

        using var context = new WmsDbContext(options);
        var entityType = context.Model.FindEntityType(
            typeof(RefreshSession));

        Assert.NotNull(entityType);
        Assert.Equal("refresh_sessions", entityType.GetTableName());
        Assert.Equal("auth", entityType.GetSchema());

        var tokenHashIndex = entityType
            .GetIndexes()
            .Single(index => index.Properties
                .Select(property => property.Name)
                .SequenceEqual([nameof(RefreshSession.TokenHash)]));

        Assert.True(tokenHashIndex.IsUnique);
    }
}
