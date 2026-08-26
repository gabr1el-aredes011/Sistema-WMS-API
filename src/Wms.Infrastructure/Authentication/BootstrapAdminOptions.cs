using System.ComponentModel.DataAnnotations;

namespace Wms.Infrastructure.Authentication;

public sealed class BootstrapAdminOptions
{
    public const string SectionName = "BootstrapAdmin";

    public bool Enabled { get; set; }

    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(160)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
