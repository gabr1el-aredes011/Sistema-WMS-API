using Wms.Domain.Shipping;

namespace Wms.Application.Shipping;

public interface IShippingService
{
    Task<IReadOnlyCollection<CarrierSummary>> GetCarriersAsync(bool? isActive, CancellationToken cancellationToken = default);
    Task<ShippingResult<CarrierSummary>> CreateCarrierAsync(CreateCarrierCommand command, CancellationToken cancellationToken = default);
    Task<ShippingResult<CarrierSummary>> SetCarrierStatusAsync(Guid carrierId, bool isActive, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PickupSummary>> GetPickupsAsync(PickupStatus? status, CancellationToken cancellationToken = default);
    Task<ShippingResult<PickupSummary>> CreatePickupAsync(CreatePickupCommand command, CancellationToken cancellationToken = default);
    Task<ShippingResult<PickupSummary>> SetPickupStatusAsync(Guid pickupId, PickupStatus status, CancellationToken cancellationToken = default);
    Task<PublicPickupView?> GetPublicPickupAsync(Guid accessToken, CancellationToken cancellationToken = default);
}
