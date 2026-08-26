using Wms.Application.Authorization;

namespace Wms.UnitTests;

public sealed class AuthorizationCatalogTests
{
    [Fact]
    public void PermissionCodes_AreUniqueAndFollowTheNamingConvention()
    {
        var codes = SystemPermissions.All
            .Select(permission => permission.Code)
            .ToArray();

        Assert.Equal(
            codes.Length,
            codes.Distinct(StringComparer.Ordinal).Count());

        Assert.All(
            codes,
            code => Assert.Matches(
                "^[a-z][a-z0-9-]*(\\.[a-z][a-z0-9-]*)+$",
                code));
    }

    [Fact]
    public void Roles_ReferenceOnlyKnownPermissions()
    {
        var knownPermissionCodes = SystemPermissions.All
            .Select(permission => permission.Code)
            .ToHashSet(StringComparer.Ordinal);

        var roleNames = SystemRoles.All
            .Select(role => role.Name)
            .ToArray();

        Assert.Equal(
            roleNames.Length,
            roleNames.Distinct(StringComparer.Ordinal).Count());

        Assert.All(SystemRoles.All, role =>
        {
            Assert.NotEmpty(role.PermissionCodes);
            Assert.Equal(
                role.PermissionCodes.Count,
                role.PermissionCodes.Distinct(StringComparer.Ordinal).Count());
            Assert.All(
                role.PermissionCodes,
                permissionCode => Assert.Contains(
                    permissionCode,
                    knownPermissionCodes));
        });
    }

    [Fact]
    public void Administrator_HasEverySystemPermission()
    {
        var administrator = SystemRoles.All.Single(
            role => role.Name == SystemRoles.Administrator);

        Assert.Equal(
            SystemPermissions.All.Select(permission => permission.Code),
            administrator.PermissionCodes);
    }
}
