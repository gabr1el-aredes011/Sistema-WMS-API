using System.ComponentModel.DataAnnotations;
using Wms.Domain.Shipping;

namespace Wms.Api.Contracts.Shipping;

public sealed class CreateCarrierRequest
{
    [Required, MaxLength(180)] public string Name { get; init; } = string.Empty;
    [MaxLength(18)] public string? TaxId { get; init; }
    [MaxLength(160)] public string? ContactName { get; init; }
    [MaxLength(256)] public string? Email { get; init; }
    [MaxLength(30)] public string? Phone { get; init; }
}

public sealed class SetCarrierStatusRequest { public bool IsActive { get; init; } }

public sealed class CreatePickupRequest
{
    public Guid CarrierId { get; init; }
    [Required, MaxLength(100)] public string OrderReference { get; init; } = string.Empty;
    [MaxLength(500)] public string? Description { get; init; }
    [Range(1, 999999)] public int VolumeCount { get; init; }
    public DateTimeOffset? ScheduledAtUtc { get; init; }
}

public sealed class SetPickupStatusRequest { public PickupStatus Status { get; init; } }
