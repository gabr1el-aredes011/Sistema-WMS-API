using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wms.Application.Authentication;

namespace Wms.Infrastructure.Authentication;

internal sealed class TokenGenerator(
    IOptions<JwtOptions> options) : ITokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public AccessTokenResult CreateAccessToken(
        CurrentUser user,
        DateTimeOffset issuedAtUtc)
    {
        var expiresAtUtc = issuedAtUtc.AddMinutes(
            _options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(user.Roles.Select(
            role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(
            permission => new Claim("permission", permission)));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAtUtc.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: credentials);

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }

    public RefreshTokenResult CreateRefreshToken(
        DateTimeOffset issuedAtUtc)
    {
        var token = Base64UrlEncoder.Encode(
            RandomNumberGenerator.GetBytes(64));

        return new RefreshTokenResult(
            token,
            HashRefreshToken(token),
            issuedAtUtc.AddDays(_options.RefreshTokenDays));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
