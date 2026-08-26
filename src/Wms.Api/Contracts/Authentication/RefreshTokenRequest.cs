using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Authentication;

public sealed class RefreshTokenRequest
{
    [Required]
    [MaxLength(2048)]
    public string RefreshToken { get; init; } = string.Empty;
}
