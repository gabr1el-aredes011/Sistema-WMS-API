namespace Wms.Application.Authentication;

public sealed record LoginResult(
    AuthenticationFailureReason FailureReason,
    AuthenticationSession? Session)
{
    public bool Succeeded => Session is not null;
}
