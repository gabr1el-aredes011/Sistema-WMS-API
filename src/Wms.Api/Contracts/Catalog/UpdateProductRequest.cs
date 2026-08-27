using System.ComponentModel.DataAnnotations;

namespace Wms.Api.Contracts.Catalog;

public sealed class UpdateProductRequest
{
    public Guid CategoryId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Type { get; init; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string ItemType { get; init; } = "Component";

    [MaxLength(100)]
    public string? Model { get; init; }

    [Range(1, int.MaxValue)]
    public int? HeightMillimeters { get; init; }

    [Range(1, int.MaxValue)]
    public int? DepthMillimeters { get; init; }

    [Range(1, int.MaxValue)]
    public int? LengthMillimeters { get; init; }
}
