using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Authentication;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; init; } = string.Empty;
}
