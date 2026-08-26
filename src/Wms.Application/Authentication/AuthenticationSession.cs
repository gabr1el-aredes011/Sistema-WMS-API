namespace Wms.Application.Authentication;

public sealed record AuthenticationSession(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    CurrentUser User);
