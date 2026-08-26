namespace Wms.Application.Authentication;

public sealed record CurrentUser(
    Guid Id,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
