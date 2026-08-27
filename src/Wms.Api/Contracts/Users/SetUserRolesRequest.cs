using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Users;

public sealed class SetUserRolesRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<string> Roles { get; init; } = [];
}
