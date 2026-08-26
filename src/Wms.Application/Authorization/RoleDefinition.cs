namespace Wms.Application.Authorization;

public sealed record RoleDefinition(
    string Name,
    string Description,
    IReadOnlyCollection<string> PermissionCodes);
