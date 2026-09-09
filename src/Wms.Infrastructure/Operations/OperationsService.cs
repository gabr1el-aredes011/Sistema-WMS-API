using Microsoft.EntityFrameworkCore;
using Wms.Application.Operations;
using Wms.Domain.Operations;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Operations;

public sealed class OperationsService(WmsDbContext db) : IOperationsService
{
    public Task<List<SalesOrder>> Orders(CancellationToken ct) => db.Set<SalesOrder>().AsNoTracking()
        .Include(x => x.Lines).Include(x => x.Events).AsSplitQuery().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

    public async Task<List<StockView>> Stock(CancellationToken ct)
    {
        var variants = await db.ProductVariants.IgnoreQueryFilters().AsNoTracking().Include(x => x.Product).OrderBy(x => x.InternalCode).ToListAsync(ct);
        var balances = await db.Set<StockBalance>().AsNoTracking().ToDictionaryAsync(x => x.VariantId, ct);
        var pending = await db.Set<SalesOrder>().AsNoTracking().Where(x => x.Status != "Dispatched" && x.Status != "Cancelled")
            .SelectMany(x => x.Lines).Where(x => !x.IsCustom && x.Quantity > x.Reserved).ToListAsync(ct);
        return variants.Select(v =>
        {
            balances.TryGetValue(v.Id, out var b);
            var lines = pending.Where(x => x.VariantId == v.Id).ToList();
            return new StockView(v.Id, v.InternalCode, v.Product.Name, v.Color, v.UnitOfMeasure,
                v.IsActive && v.Product.IsActive && v.Product.DeletedAtUtc == null,
                b?.OnHand ?? 0, b?.Reserved ?? 0, lines.Sum(x => x.Quantity - x.Reserved), lines.Select(x => x.SalesOrderId).Distinct().Count());
        }).ToList();
    }

    // One transaction-scoped lock serializes reservation writes across API instances.
    // This conservative first version prioritizes consistency; later it can lock per variant.
    private Task Lock(CancellationToken ct) => db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(88421001)", ct);
    private static void Event(SalesOrder order, string actor, string message) => order.Events.Add(new OrderEvent
    { Actor = actor, Message = message, CreatedAtUtc = DateTimeOffset.UtcNow });

    public async Task Create(CreateOrderInput input, string actor, CancellationToken ct)
    {
        if (input.Id == Guid.Empty || !new[] { "Instagram", "Mercado Livre", "WhatsApp", "Site oficial", "Presencial" }.Contains(input.Channel))
            throw new ArgumentException("Identificador ou canal de venda inválido.");
        if (input.Lines.Count is < 1 or > 200 || input.Lines.Any(x => x.Quantity <= 0 || x.Quantity > 1_000_000_000 || decimal.Round(x.Quantity, 3) != x.Quantity || string.IsNullOrWhiteSpace(x.Description)))
            throw new ArgumentException("Confira os itens e as quantidades (até três casas decimais).");
        var pdf = InvoiceReader.Decode(input.PdfBase64, 5_000_000, "PDF");
        var xml = InvoiceReader.Decode(input.XmlBase64, 2_000_000, "XML");
        var attachments = new List<OrderDocument>();
        if (input.Attachments?.Count > 5) throw new ArgumentException("Limite de cinco anexos adicionais.");
        foreach (var attachment in input.Attachments ?? [])
        {
            var bytes = InvoiceReader.Decode(attachment.ContentBase64, 2_000_000, "anexo");
            var isPdf = bytes.AsSpan().StartsWith("%PDF-"u8);
            var isPng = bytes.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
            var isJpeg = bytes.AsSpan().StartsWith(new byte[] { 255, 216, 255 });
            if (!isPdf && !isPng && !isJpeg) throw new ArgumentException("Anexos adicionais devem ser PDF, PNG ou JPEG.");
            var extension = isPdf ? ".pdf" : isPng ? ".png" : ".jpg";
            attachments.Add(new OrderDocument { SalesOrderId = input.Id, Kind = "extra-" + Guid.NewGuid(), Name = Path.GetFileNameWithoutExtension(attachment.Name) + extension, Content = bytes });
        }
        (string Key, string Number, List<InvoiceReader.InvoiceItem> Items) invoice;
        try { invoice = InvoiceReader.Read(xml); }
        catch (System.Xml.XmlException) { throw new ArgumentException("XML de NF-e inválido ou inseguro."); }
        if (invoice.Items.Count != input.Lines.Count) throw new ArgumentException("Mapeie todos os itens da NF-e antes de cadastrar o pedido.");
        for (var i = 0; i < input.Lines.Count; i++)
        {
            if (invoice.Items[i].Quantity != input.Lines[i].Quantity || invoice.Items[i].Description.Trim() != input.Lines[i].Description.Trim())
                throw new ArgumentException("Descrição e quantidade devem corresponder a cada item do XML.");
            if (!input.Lines[i].IsCustom)
            {
                var unit = await db.ProductVariants.Where(x => x.Id == input.Lines[i].VariantId).Select(x => x.UnitOfMeasure).FirstOrDefaultAsync(ct);
                if (!string.Equals(unit, invoice.Items[i].Unit.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Unidade incompatível no item {i + 1}: a NF-e usa {invoice.Items[i].Unit}. Conversão de embalagens deve ser validada antes da reserva.");
            }
        }
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await Lock(ct);
        var previousOrder = await db.Set<SalesOrder>().FirstOrDefaultAsync(x => x.Id == input.Id, ct);
        if (previousOrder != null)
        {
            if (previousOrder.InvoiceKey != invoice.Key) throw new ArgumentException("Identificador já utilizado por outra nota.");
            return;
        }
        if (await db.Set<SalesOrder>().AnyAsync(x => x.InvoiceKey == invoice.Key, ct)) throw new ArgumentException("Esta NF-e já possui um pedido cadastrado.");
        foreach (var line in input.Lines)
        {
            if (!line.IsCustom && line.VariantId == null) throw new ArgumentException("Vincule cada item de catálogo a uma variante.");
            if (line.VariantId != null) await Active(line.VariantId.Value, ct);
            if (line.IsCustom && string.IsNullOrWhiteSpace(line.Details)) throw new ArgumentException("Descreva os detalhes do produto personalizado.");
        }
        var order = new SalesOrder
        {
            Id = input.Id, InvoiceKey = invoice.Key, InvoiceNumber = invoice.Number, Customer = input.Customer.Trim(),
            Channel = input.Channel, CreatedAtUtc = DateTimeOffset.UtcNow, CreatedBy = actor, DueAtUtc = input.DueAtUtc,
            Lines = input.Lines.Select(x => new OrderLine { VariantId = x.VariantId, Description = x.Description.Trim(), Quantity = x.Quantity, IsCustom = x.IsCustom, Details = x.Details }).ToList()
        };
        Event(order, actor, "Pedido cadastrado; reserva automática iniciada.");
        db.Add(order);
        db.AddRange(new OrderDocument { SalesOrderId = order.Id, Kind = "pdf", Name = "Nota fiscal.pdf", Content = pdf }, new OrderDocument { SalesOrderId = order.Id, Kind = "xml", Name = "Nota fiscal.xml", Content = xml });
        db.AddRange(attachments);
        await db.SaveChangesAsync(ct);
        await Allocate(ct);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    private async Task Active(Guid id, CancellationToken ct)
    {
        var variant = await db.ProductVariants.IgnoreQueryFilters().Include(x => x.Product).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (variant == null) throw new ArgumentException("Código não encontrado.");
        if (!variant.IsActive || !variant.Product.IsActive || variant.Product.DeletedAtUtc != null)
            throw new ArgumentException("Produto Inativado do Sistema");
    }

    private async Task Allocate(CancellationToken ct)
    {
        var balances = await db.Set<StockBalance>().ToDictionaryAsync(x => x.VariantId, ct);
        var active = await db.ProductVariants.Where(x => x.IsActive && x.Product.IsActive && x.Product.DeletedAtUtc == null).Select(x => x.Id).ToListAsync(ct);
        var orders = await db.Set<SalesOrder>().Include(x => x.Lines).Where(x => x.Status == "Preparing" || x.Status == "Separating")
            .OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id).ToListAsync(ct);
        foreach (var order in orders)
        foreach (var line in order.Lines.Where(x => !x.IsCustom).OrderBy(x => x.Id))
        {
            if (line.VariantId is not Guid id || !active.Contains(id) || !balances.TryGetValue(id, out var b)) continue;
            var quantity = ReservationRules.Allocate(b.OnHand - b.Reserved, line.Quantity - line.Reserved);
            if (quantity <= 0) continue;
            line.Reserved += quantity; b.Reserved += quantity;
            Event(order, "Sistema", $"Reserva completada em {quantity}: {line.Description}.");
        }
    }

    public async Task Produce(ProductionInput input, string actor, CancellationToken ct)
    {
        if (input.Id == Guid.Empty || input.Quantity <= 0 || input.Quantity > 1_000_000_000 || decimal.Round(input.Quantity, 3) != input.Quantity)
            throw new ArgumentException("Informe uma quantidade positiva com até três casas decimais.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await Lock(ct);
        var previous = await db.Set<StockMovement>().FirstOrDefaultAsync(x => x.OperationId == input.Id, ct);
        if (previous != null)
        {
            if (previous.Kind != "Production" || previous.VariantId != input.VariantId || previous.Quantity != input.Quantity)
                throw new ArgumentException("Identificador de operação já utilizado com outros dados.");
            return;
        }
        await Active(input.VariantId, ct);
        var balance = await db.Set<StockBalance>().FindAsync([input.VariantId], ct);
        if (balance == null) { balance = new StockBalance { VariantId = input.VariantId }; db.Add(balance); }
        balance.OnHand += input.Quantity;
        db.Add(new StockMovement { OperationId = input.Id, VariantId = input.VariantId, Quantity = input.Quantity, Actor = actor, CreatedAtUtc = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync(ct);
        await Allocate(ct);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task Act(Guid id, OrderActionInput input, string actor, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await Lock(ct);
        var order = await db.Set<SalesOrder>().Include(x => x.Lines).Include(x => x.Events).SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new ArgumentException("Pedido não encontrado.");
        if (order.Status is "Cancelled" or "Dispatched") throw new ArgumentException("Pedido encerrado; seu histórico está preservado.");
        if (input.Action == "Comment")
        {
            if (string.IsNullOrWhiteSpace(input.Message)) throw new ArgumentException("Informe o comentário.");
            Event(order, actor, input.Message.Trim());
        }
        else if (input.Action == "Assign")
        {
            if (string.IsNullOrWhiteSpace(input.Responsible)) throw new ArgumentException("Informe o responsável.");
            order.Responsible = input.Responsible.Trim(); Event(order, actor, $"Responsável: {order.Responsible}.");
        }
        else if (input.Action == "CustomComplete")
        {
            var line = order.Lines.SingleOrDefault(x => x.Id == input.LineId && x.IsCustom) ?? throw new ArgumentException("Item personalizado não encontrado.");
            line.CustomCompleted = true; Event(order, actor, $"Produção personalizada concluída: {line.Description}.");
        }
        else
        {
            if (!ReservationRules.CanTransition(order.Status, input.Action)) throw new ArgumentException("Transição de etapa não permitida.");
            if (input.Action is "Ready" or "Dispatched")
            {
                if (!ReservationRules.IsComplete(order.Lines)) throw new ArgumentException("Entrega parcial bloqueada: há produtos em preparação/separação.");
                foreach (var line in order.Lines.Where(x => !x.IsCustom)) await Active(line.VariantId!.Value, ct);
            }
            if (input.Action == "Ready" && (!input.ConferenceConfirmed || string.IsNullOrWhiteSpace(order.Responsible)))
                throw new ArgumentException("Defina o responsável e confirme conferência e embalagem de todos os itens.");
            if (input.Action is "Dispatched" or "Cancelled")
            {
                foreach (var group in order.Lines.Where(x => !x.IsCustom).GroupBy(x => x.VariantId!.Value))
                {
                    var quantity = group.Sum(x => x.Reserved);
                    var b = await db.Set<StockBalance>().FindAsync([group.Key], ct);
                    if (b == null && quantity == 0) continue;
                    if (b == null || b.Reserved < quantity) throw new ArgumentException("Saldo inconsistente; revise a reserva.");
                    b.Reserved -= quantity;
                    if (input.Action == "Dispatched")
                    {
                        b.OnHand -= quantity;
                        db.Add(new StockMovement { OperationId = order.Id, VariantId = group.Key, Quantity = -quantity, Kind = "Dispatch", Actor = actor, CreatedAtUtc = DateTimeOffset.UtcNow });
                    }
                    foreach (var line in group) line.Reserved = 0;
                }
            }
            var label = input.Action switch { "Separating" => "Em separação", "Ready" => "Pronto para retirada", "Dispatched" => "Expedido", "Cancelled" => "Cancelado", _ => input.Action };
            order.Status = input.Action; Event(order, actor, $"Etapa alterada para {label}.");
        }
        await db.SaveChangesAsync(ct);
        await Allocate(ct);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public Task<byte[]?> Document(Guid id, string kind, CancellationToken ct) => db.Set<OrderDocument>()
        .Where(x => x.SalesOrderId == id && x.Kind == kind).Select(x => x.Content).SingleOrDefaultAsync(ct);
    public Task<List<DocumentView>> Documents(Guid id, CancellationToken ct) => db.Set<OrderDocument>()
        .Where(x => x.SalesOrderId == id).Select(x => new DocumentView(x.Kind, x.Name)).ToListAsync(ct);
}
