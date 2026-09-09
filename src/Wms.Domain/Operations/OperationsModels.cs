namespace Wms.Domain.Operations;

public sealed class SalesOrder
{
    public Guid Id { get; set; }
    public string InvoiceKey { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public string Channel { get; set; } = "";
    public string Customer { get; set; } = "";
    public string Status { get; set; } = "Preparing";
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string CreatedBy { get; set; } = "";
    public string? Responsible { get; set; }
    public DateTimeOffset? DueAtUtc { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
    public List<OrderEvent> Events { get; set; } = [];
}

public sealed class OrderLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SalesOrderId { get; set; }
    public Guid? VariantId { get; set; }
    public string Description { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal Reserved { get; set; }
    public bool IsCustom { get; set; }
    public bool CustomCompleted { get; set; }
    public string? Details { get; set; }
}

public sealed class StockBalance
{
    public Guid VariantId { get; set; }
    public decimal OnHand { get; set; }
    public decimal Reserved { get; set; }
}

public sealed class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OperationId { get; set; }
    public Guid VariantId { get; set; }
    public decimal Quantity { get; set; }
    public string Kind { get; set; } = "Production";
    public string Actor { get; set; } = "";
    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class OrderEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SalesOrderId { get; set; }
    public string Actor { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class OrderDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SalesOrderId { get; set; }
    public string Kind { get; set; } = "";
    public string Name { get; set; } = "";
    public byte[] Content { get; set; } = [];
}

public static class ReservationRules
{
    public static decimal Allocate(decimal available, decimal missing) => Math.Max(0, Math.Min(available, missing));
    public static bool IsComplete(IEnumerable<OrderLine> lines) => lines.Any() &&
        lines.All(x => x.IsCustom ? x.CustomCompleted : x.Reserved == x.Quantity);
    public static bool CanTransition(string from, string to) => (from, to) switch
    {
        ("Preparing", "Separating") or ("Separating", "Ready") or ("Ready", "Dispatched") => true,
        ("Preparing" or "Separating" or "Ready", "Cancelled") => true,
        _ => false
    };
}
