using Wms.Application.Authorization;

namespace Wms.Infrastructure.Persistence.Seed;

internal static class AuthorizationSeedData
{
    internal static readonly DateTimeOffset SeededAtUtc =
        new(2026, 8, 26, 0, 0, 0, TimeSpan.Zero);

    private static readonly Guid[] PermissionIds =
    [
        Guid.Parse("10000000-0000-0000-0000-000000000001"),
        Guid.Parse("10000000-0000-0000-0000-000000000002"),
        Guid.Parse("10000000-0000-0000-0000-000000000003"),
        Guid.Parse("10000000-0000-0000-0000-000000000004"),
        Guid.Parse("10000000-0000-0000-0000-000000000005"),
        Guid.Parse("10000000-0000-0000-0000-000000000006"),
        Guid.Parse("10000000-0000-0000-0000-000000000007"),
        Guid.Parse("10000000-0000-0000-0000-000000000008"),
        Guid.Parse("10000000-0000-0000-0000-000000000009"),
        Guid.Parse("10000000-0000-0000-0000-000000000010"),
        Guid.Parse("10000000-0000-0000-0000-000000000011"),
        Guid.Parse("10000000-0000-0000-0000-000000000012"),
        Guid.Parse("10000000-0000-0000-0000-000000000013"),
        Guid.Parse("10000000-0000-0000-0000-000000000014"),
        Guid.Parse("10000000-0000-0000-0000-000000000015"),
        Guid.Parse("10000000-0000-0000-0000-000000000016"),
        Guid.Parse("10000000-0000-0000-0000-000000000017"),
        Guid.Parse("10000000-0000-0000-0000-000000000018"),
        Guid.Parse("10000000-0000-0000-0000-000000000019"),
        Guid.Parse("10000000-0000-0000-0000-000000000020"),
        Guid.Parse("10000000-0000-0000-0000-000000000021"),
        Guid.Parse("10000000-0000-0000-0000-000000000022"),
        Guid.Parse("10000000-0000-0000-0000-000000000023"),
        Guid.Parse("10000000-0000-0000-0000-000000000024"),
        Guid.Parse("10000000-0000-0000-0000-000000000025"),
        Guid.Parse("10000000-0000-0000-0000-000000000026"),
        Guid.Parse("10000000-0000-0000-0000-000000000027")
    ];

    private static readonly Guid[] RoleIds =
    [
        Guid.Parse("20000000-0000-0000-0000-000000000001"),
        Guid.Parse("20000000-0000-0000-0000-000000000002"),
        Guid.Parse("20000000-0000-0000-0000-000000000003"),
        Guid.Parse("20000000-0000-0000-0000-000000000004"),
        Guid.Parse("20000000-0000-0000-0000-000000000005"),
        Guid.Parse("20000000-0000-0000-0000-000000000006")
    ];

    internal static IReadOnlyList<PermissionSeed> Permissions { get; } =
        SystemPermissions.All
            .Select((definition, index) =>
                new PermissionSeed(PermissionIds[index], definition))
            .ToArray();

    internal static IReadOnlyList<RoleSeed> Roles { get; } =
        SystemRoles.All
            .Select((definition, index) =>
                new RoleSeed(RoleIds[index], definition))
            .ToArray();

    internal static IReadOnlyList<RolePermissionSeed> RolePermissions { get; } =
        CreateRolePermissions();

    private static IReadOnlyList<RolePermissionSeed> CreateRolePermissions()
    {
        var permissionIdsByCode = Permissions.ToDictionary(
            permission => permission.Definition.Code,
            permission => permission.Id,
            StringComparer.Ordinal);

        return Roles
            .SelectMany(role => role.Definition.PermissionCodes.Select(
                permissionCode => new RolePermissionSeed(
                    role.Id,
                    permissionIdsByCode[permissionCode])))
            .ToArray();
    }

    internal sealed record PermissionSeed(
        Guid Id,
        PermissionDefinition Definition);

    internal sealed record RoleSeed(
        Guid Id,
        RoleDefinition Definition);

    internal sealed record RolePermissionSeed(
        Guid RoleId,
        Guid PermissionId);
}
