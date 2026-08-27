using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Catalog;

public sealed class CreateProductVariantRequest
{
    [Required]
    [MaxLength(60)]
    public string Color { get; init; } = string.Empty;

    [MaxLength(64)]
    public string? ExternalReference { get; init; }

    [MaxLength(64)]
    public string? ExternalBarcode { get; init; }

    [Required]
    [MaxLength(12)]
    public string UnitOfMeasure { get; init; } = "UN";
}
