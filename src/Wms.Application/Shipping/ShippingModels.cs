using Wms.Domain.Shipping;

namespace Wms.Application.Shipping;

public sealed record CarrierSummary(Guid Id, string Name, string? TaxId, string? ContactName, string? Email, string? Phone, bool IsActive);
public sealed record PickupSummary(Guid Id, string Code, Guid CarrierId, string CarrierName, string OrderReference, string? Description, int VolumeCount, DateTimeOffset? ScheduledAtUtc, PickupStatus Status, Guid PublicAccessToken, DateTimeOffset CreatedAtUtc, DateTimeOffset? ReadyAtUtc, DateTimeOffset? CollectedAtUtc);
public sealed record PublicPickupView(string Code, string CarrierName, string OrderReference, string? Description, int VolumeCount, DateTimeOffset? ScheduledAtUtc, PickupStatus Status, DateTimeOffset? ReadyAtUtc, DateTimeOffset? CollectedAtUtc, DateTimeOffset? UpdatedAtUtc);
public sealed record CreateCarrierCommand(string Name, string? TaxId, string? ContactName, string? Email, string? Phone);
public sealed record CreatePickupCommand(Guid CarrierId, string OrderReference, string? Description, int VolumeCount, DateTimeOffset? ScheduledAtUtc);

public sealed record ShippingResult<T>(T? Value, string[] Errors)
{
    public bool Succeeded => Value is not null;
    public static ShippingResult<T> Success(T value) => new(value, []);
    public static ShippingResult<T> Fail(params string[] errors) => new(default, errors);
}
