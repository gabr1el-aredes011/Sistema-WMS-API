namespace Wms.Api.Contracts.Users;

public sealed class SetUserStatusRequest
{
    public bool IsActive { get; init; }
}
