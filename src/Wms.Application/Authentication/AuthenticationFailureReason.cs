namespace Wms.Application.Authentication;

public enum AuthenticationFailureReason
{
    None = 0,
    InvalidCredentials = 1,
    LockedOut = 2,
    InactiveUser = 3
}
