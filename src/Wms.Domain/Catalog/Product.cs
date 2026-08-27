namespace Wms.Domain.Catalog;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CategoryId { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ItemType { get; set; } = CatalogItemTypes.Component;

    public string? Model { get; set; }

    public int? HeightMillimeters { get; set; }

    public int? DepthMillimeters { get; set; }

    public int? LengthMillimeters { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}
