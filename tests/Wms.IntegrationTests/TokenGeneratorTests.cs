using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wms.Application.Authentication;
using Wms.Infrastructure.Authentication;

namespace Wms.IntegrationTests;

public sealed class TokenGeneratorTests
{
    private const string SigningKey =
        "wms-integration-tests-signing-key-with-at-least-32-bytes";

    private static TokenGenerator CreateGenerator()
    {
        return new TokenGenerator(Options.Create(new JwtOptions
        {
            Issuer = "Wms.Tests",
            Audience = "Wms.Tests.Client",
            SigningKey = SigningKey,
            AccessTokenMinutes = 15,
            RefreshTokenDays = 7
        }));
    }

    [Fact]
    public void RefreshTokens_AreRandomHashedAndHaveTheExpectedLifetime()
    {
        var generator = CreateGenerator();
        var now = new DateTimeOffset(
            2026,
            8,
            26,
            12,
            0,
            0,
            TimeSpan.Zero);

        var first = generator.CreateRefreshToken(now);
        var second = generator.CreateRefreshToken(now);

        Assert.NotEqual(first.Token, second.Token);
        Assert.NotEqual(first.TokenHash, second.TokenHash);
        Assert.Equal(64, first.TokenHash.Length);
        Assert.Equal(first.TokenHash, generator.HashRefreshToken(first.Token));
        Assert.Equal(now.AddDays(7), first.ExpiresAtUtc);
    }

    [Fact]
    public void AccessToken_IsSignedAndContainsIdentityAuthorizationClaims()
    {
        var generator = CreateGenerator();
        var now = DateTimeOffset.UtcNow;
        var user = new CurrentUser(
            Guid.NewGuid(),
            "WMS Administrator",
            "admin@wms.local",
            ["Administrator"],
            ["inventory.read", "users.read"]);

        var result = generator.CreateAccessToken(user, now);
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var principal = tokenHandler.ValidateToken(
            result.Token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "Wms.Tests",
                ValidateAudience = true,
                ValidAudience = "Wms.Tests.Client",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            },
            out _);

        Assert.Equal(user.Id.ToString(), principal.FindFirst("sub")?.Value);
        Assert.True(principal.IsInRole("Administrator"));
        Assert.Contains(
            principal.FindAll("permission").Select(claim => claim.Value),
            permission => permission == "inventory.read");
        Assert.Equal(now.AddMinutes(15), result.ExpiresAtUtc);
    }
}
