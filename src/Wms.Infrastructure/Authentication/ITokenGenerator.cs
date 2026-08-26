using Wms.Application.Authentication;

namespace Wms.Infrastructure.Authentication;

internal interface ITokenGenerator
{
    AccessTokenResult CreateAccessToken(
        CurrentUser user,
        DateTimeOffset issuedAtUtc);

    RefreshTokenResult CreateRefreshToken(
        DateTimeOffset issuedAtUtc);

    string HashRefreshToken(string refreshToken);
}

internal sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);

internal sealed record RefreshTokenResult(
    string Token,
    string TokenHash,
    DateTimeOffset ExpiresAtUtc);
