using Wms.Infrastructure.Identity;

namespace Wms.Infrastructure.Authorization;

public sealed class RolePermission
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTimeOffset AssignedAtUtc { get; set; }
        = DateTimeOffset.UtcNow;

    public Guid? AssignedByUserId { get; set; }

    public ApplicationRole Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;

    public ApplicationUser? AssignedByUser { get; set; }
}
