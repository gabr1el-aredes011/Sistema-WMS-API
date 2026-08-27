namespace Wms.Domain.Catalog;

public sealed class ProductVariant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string InternalCode { get; private set; } = null!;

    public string Color { get; set; } = string.Empty;

    public string? ExternalReference { get; set; }

    public string? ExternalBarcode { get; set; }

    public string UnitOfMeasure { get; set; } = "UN";

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }
}
