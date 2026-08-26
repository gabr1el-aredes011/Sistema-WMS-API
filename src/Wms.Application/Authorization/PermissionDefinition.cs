namespace Wms.Application.Authorization;

public sealed record PermissionDefinition(
    string Code,
    string Name,
    string Module,
    string Description);
