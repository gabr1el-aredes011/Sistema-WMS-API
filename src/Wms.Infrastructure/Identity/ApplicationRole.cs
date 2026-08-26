using Microsoft.AspNetCore.Identity;
using Wms.Infrastructure.Authorization;

namespace Wms.Infrastructure.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
        = DateTimeOffset.UtcNow;

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}
