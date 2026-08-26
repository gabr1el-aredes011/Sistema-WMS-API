using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Users;

public sealed class UpdateUserRequest
{
    [Required]
    [MaxLength(160)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;
}
