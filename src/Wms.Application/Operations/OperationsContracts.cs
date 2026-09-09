using System.ComponentModel.DataAnnotations;
using Wms.Domain.Operations;

namespace Wms.Application.Operations;

public sealed record OrderLineInput(Guid? VariantId, [Required, MaxLength(300)] string Description,
    [Range(typeof(decimal), "0.001", "1000000000")] decimal Quantity, bool IsCustom, [MaxLength(2000)] string? Details);
public sealed record CreateOrderInput(Guid Id, [Required] string Channel, [Required, MaxLength(200)] string Customer,
    DateTimeOffset? DueAtUtc, [Required, MinLength(1), MaxLength(200)] List<OrderLineInput> Lines,
    [Required] string PdfBase64, [Required] string XmlBase64, [MaxLength(5)] List<AttachmentInput>? Attachments = null);
public sealed record AttachmentInput([Required, MaxLength(120)] string Name, [Required] string ContentBase64);
public sealed record DocumentView(string Kind, string Name);
public sealed record ProductionInput(Guid Id, Guid VariantId, [Range(typeof(decimal), "0.001", "1000000000")] decimal Quantity);
public sealed record OrderActionInput([Required] string Action, [MaxLength(2000)] string? Message,
    [MaxLength(150)] string? Responsible, Guid? LineId, bool ConferenceConfirmed = false);
public sealed record StockView(Guid VariantId, string Code, string Product, string Color, string Unit,
    bool IsActive, decimal OnHand, decimal Reserved, decimal Missing, int PendingOrders);
public interface IOperationsService
{
    Task<List<SalesOrder>> Orders(CancellationToken ct);
    Task<List<StockView>> Stock(CancellationToken ct);
    Task Create(CreateOrderInput input, string actor, CancellationToken ct);
    Task Produce(ProductionInput input, string actor, CancellationToken ct);
    Task Act(Guid id, OrderActionInput input, string actor, CancellationToken ct);
    Task<byte[]?> Document(Guid id, string kind, CancellationToken ct);
    Task<List<DocumentView>> Documents(Guid id, CancellationToken ct);
}
