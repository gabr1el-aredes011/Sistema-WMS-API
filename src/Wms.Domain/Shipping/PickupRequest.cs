namespace Wms.Domain.Shipping;

public enum PickupStatus
{
    Preparing = 0,
    ReadyForPickup = 1,
    Collected = 2,
    Cancelled = 3
}

public sealed class PickupRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public Guid CarrierId { get; set; }
    public Carrier Carrier { get; set; } = null!;
    public string OrderReference { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int VolumeCount { get; set; }
    public DateTimeOffset? ScheduledAtUtc { get; set; }
    public PickupStatus Status { get; set; } = PickupStatus.Preparing;
    public Guid PublicAccessToken { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public DateTimeOffset? ReadyAtUtc { get; set; }
    public DateTimeOffset? CollectedAtUtc { get; set; }
}
