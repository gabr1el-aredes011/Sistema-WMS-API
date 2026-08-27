namespace Wms.Domain.Suppliers;

public sealed class Supplier
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string LegalName { get; set; } = string.Empty;

    public string NormalizedLegalName { get; set; } = string.Empty;

    public string? TradeName { get; set; }

    public string TaxId { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }
}
