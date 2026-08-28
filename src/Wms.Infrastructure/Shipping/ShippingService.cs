using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Wms.Application.Shipping;
using Wms.Domain.Shipping;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Shipping;

internal sealed class ShippingService(WmsDbContext dbContext, TimeProvider timeProvider) : IShippingService
{
    public async Task<IReadOnlyCollection<CarrierSummary>> GetCarriersAsync(bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Carriers.AsNoTracking();
        if (isActive.HasValue) query = query.Where(item => item.IsActive == isActive);
        return await query.OrderBy(item => item.Name).Select(item => MapCarrier(item)).ToArrayAsync(cancellationToken);
    }

    public async Task<ShippingResult<CarrierSummary>> CreateCarrierAsync(CreateCarrierCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) return ShippingResult<CarrierSummary>.Fail("Informe o nome da transportadora.");
        if (!string.IsNullOrWhiteSpace(command.Email) && !MailAddress.TryCreate(command.Email.Trim(), out _)) return ShippingResult<CarrierSummary>.Fail("Informe um e-mail válido.");
        var normalized = command.Name.Trim().ToUpperInvariant();
        if (await dbContext.Carriers.AnyAsync(item => item.NormalizedName == normalized, cancellationToken)) return ShippingResult<CarrierSummary>.Fail("Esta transportadora já está cadastrada.");
        var taxId = DigitsOnly(command.TaxId);
        if (taxId is not null && taxId.Length != 14) return ShippingResult<CarrierSummary>.Fail("O CNPJ deve possuir 14 dígitos.");
        if (taxId is not null && await dbContext.Carriers.AnyAsync(item => item.TaxId == taxId, cancellationToken)) return ShippingResult<CarrierSummary>.Fail("Este CNPJ já está cadastrado.");
        var carrier = new Carrier { Name = command.Name.Trim(), NormalizedName = normalized, TaxId = taxId, ContactName = Optional(command.ContactName), Email = Optional(command.Email)?.ToLowerInvariant(), Phone = Optional(command.Phone), CreatedAtUtc = timeProvider.GetUtcNow() };
        dbContext.Carriers.Add(carrier);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ShippingResult<CarrierSummary>.Success(MapCarrier(carrier));
    }

    public async Task<ShippingResult<CarrierSummary>> SetCarrierStatusAsync(Guid carrierId, bool isActive, CancellationToken cancellationToken = default)
    {
        var carrier = await dbContext.Carriers.FindAsync([carrierId], cancellationToken);
        if (carrier is null) return ShippingResult<CarrierSummary>.Fail("Transportadora não encontrada.");
        carrier.IsActive = isActive; carrier.UpdatedAtUtc = timeProvider.GetUtcNow();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ShippingResult<CarrierSummary>.Success(MapCarrier(carrier));
    }

    public async Task<IReadOnlyCollection<PickupSummary>> GetPickupsAsync(PickupStatus? status, CancellationToken cancellationToken = default)
    {
        var query = dbContext.PickupRequests.AsNoTracking();
        if (status.HasValue) query = query.Where(item => item.Status == status);
        return await query.OrderByDescending(item => item.CreatedAtUtc).Select(item => new PickupSummary(item.Id, item.Code, item.CarrierId, item.Carrier.Name, item.OrderReference, item.Description, item.VolumeCount, item.ScheduledAtUtc, item.Status, item.PublicAccessToken, item.CreatedAtUtc, item.ReadyAtUtc, item.CollectedAtUtc)).ToArrayAsync(cancellationToken);
    }

    public async Task<ShippingResult<PickupSummary>> CreatePickupAsync(CreatePickupCommand command, CancellationToken cancellationToken = default)
    {
        var carrier = await dbContext.Carriers.SingleOrDefaultAsync(item => item.Id == command.CarrierId && item.IsActive, cancellationToken);
        if (carrier is null) return ShippingResult<PickupSummary>.Fail("Selecione uma transportadora ativa.");
        if (string.IsNullOrWhiteSpace(command.OrderReference)) return ShippingResult<PickupSummary>.Fail("Informe a referência do pedido ou da carga.");
        if (command.VolumeCount < 1) return ShippingResult<PickupSummary>.Fail("Informe ao menos um volume.");
        var pickup = new PickupRequest { CarrierId = carrier.Id, Carrier = carrier, OrderReference = command.OrderReference.Trim(), Description = Optional(command.Description), VolumeCount = command.VolumeCount, ScheduledAtUtc = command.ScheduledAtUtc, CreatedAtUtc = timeProvider.GetUtcNow() };
        dbContext.PickupRequests.Add(pickup);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ShippingResult<PickupSummary>.Success(MapPickup(pickup));
    }

    public async Task<ShippingResult<PickupSummary>> SetPickupStatusAsync(Guid pickupId, PickupStatus status, CancellationToken cancellationToken = default)
    {
        var pickup = await dbContext.PickupRequests.Include(item => item.Carrier).SingleOrDefaultAsync(item => item.Id == pickupId, cancellationToken);
        if (pickup is null) return ShippingResult<PickupSummary>.Fail("Solicitação de coleta não encontrada.");
        if (!IsTransitionAllowed(pickup.Status, status)) return ShippingResult<PickupSummary>.Fail("Esta mudança de status não é permitida.");
        var now = timeProvider.GetUtcNow(); pickup.Status = status; pickup.UpdatedAtUtc = now;
        if (status == PickupStatus.ReadyForPickup) pickup.ReadyAtUtc = now;
        if (status == PickupStatus.Collected) pickup.CollectedAtUtc = now;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ShippingResult<PickupSummary>.Success(MapPickup(pickup));
    }

    public async Task<PublicPickupView?> GetPublicPickupAsync(Guid accessToken, CancellationToken cancellationToken = default) =>
        await dbContext.PickupRequests.AsNoTracking().Where(item => item.PublicAccessToken == accessToken).Select(item => new PublicPickupView(item.Code, item.Carrier.Name, item.OrderReference, item.Description, item.VolumeCount, item.ScheduledAtUtc, item.Status, item.ReadyAtUtc, item.CollectedAtUtc, item.UpdatedAtUtc)).SingleOrDefaultAsync(cancellationToken);

    private static bool IsTransitionAllowed(PickupStatus current, PickupStatus next) => current == next || (current, next) is (PickupStatus.Preparing, PickupStatus.ReadyForPickup) or (PickupStatus.Preparing, PickupStatus.Cancelled) or (PickupStatus.ReadyForPickup, PickupStatus.Collected) or (PickupStatus.ReadyForPickup, PickupStatus.Cancelled);
    private static CarrierSummary MapCarrier(Carrier item) => new(item.Id, item.Name, item.TaxId, item.ContactName, item.Email, item.Phone, item.IsActive);
    private static PickupSummary MapPickup(PickupRequest item) => new(item.Id, item.Code, item.CarrierId, item.Carrier.Name, item.OrderReference, item.Description, item.VolumeCount, item.ScheduledAtUtc, item.Status, item.PublicAccessToken, item.CreatedAtUtc, item.ReadyAtUtc, item.CollectedAtUtc);
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? DigitsOnly(string? value) { if (string.IsNullOrWhiteSpace(value)) return null; return new string(value.Where(char.IsDigit).ToArray()); }
}
