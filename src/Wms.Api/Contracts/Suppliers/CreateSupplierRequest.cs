using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Suppliers;

public sealed class CreateSupplierRequest
{
    [Required]
    [MaxLength(200)]
    public string LegalName { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? TradeName { get; init; }

    [Required]
    [MaxLength(18)]
    public string TaxId { get; init; } = string.Empty;

    [EmailAddress]
    [MaxLength(256)]
    public string? Email { get; init; }

    [MaxLength(30)]
    public string? Phone { get; init; }
}
