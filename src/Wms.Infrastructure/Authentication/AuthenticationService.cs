using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wms.Application.Authentication;
using Wms.Infrastructure.Identity;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure.Authentication;

internal sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    WmsDbContext dbContext,
    ITokenGenerator tokenGenerator,
    TimeProvider timeProvider) : IAuthenticationService
{
    public async Task<LoginResult> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new LoginResult(
                AuthenticationFailureReason.InvalidCredentials,
                null);
        }

        if (!user.IsActive)
        {
            return new LoginResult(
                AuthenticationFailureReason.InactiveUser,
                null);
        }

        var passwordResult = await signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        if (passwordResult.IsLockedOut)
        {
            return new LoginResult(
                AuthenticationFailureReason.LockedOut,
                null);
        }

        if (!passwordResult.Succeeded)
        {
            return new LoginResult(
                AuthenticationFailureReason.InvalidCredentials,
                null);
        }

        var now = timeProvider.GetUtcNow();
        var currentUser = await BuildCurrentUserAsync(
            user,
            cancellationToken);
        var session = CreateSession(
            user,
            currentUser,
            now,
            ipAddress,
            userAgent);

        user.LastLoginAtUtc = now;
        dbContext.RefreshSessions.Add(session.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResult(
            AuthenticationFailureReason.None,
            session.Response);
    }

    public async Task<AuthenticationSession?> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = tokenGenerator.HashRefreshToken(refreshToken);
        var now = timeProvider.GetUtcNow();

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var existingSession = await dbContext.RefreshSessions
            .Include(session => session.User)
            .SingleOrDefaultAsync(
                session => session.TokenHash == tokenHash,
                cancellationToken);

        if (existingSession is null)
        {
            return null;
        }

        if (existingSession.RevokedAtUtc is not null)
        {
            await RevokeActiveSessionsAsync(
                existingSession.UserId,
                now,
                ipAddress,
                "RefreshTokenReuseDetected",
                cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        if (existingSession.ExpiresAtUtc <= now ||
            !existingSession.User.IsActive)
        {
            return null;
        }

        var currentUser = await BuildCurrentUserAsync(
            existingSession.User,
            cancellationToken);
        var replacement = CreateSession(
            existingSession.User,
            currentUser,
            now,
            ipAddress,
            userAgent);

        existingSession.LastUsedAtUtc = now;
        existingSession.RevokedAtUtc = now;
        existingSession.RevokedByIp = Normalize(ipAddress, 64);
        existingSession.RevocationReason = "Rotated";
        existingSession.ReplacedBySessionId = replacement.Entity.Id;

        dbContext.RefreshSessions.Add(replacement.Entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return replacement.Response;
    }

    public async Task LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = tokenGenerator.HashRefreshToken(refreshToken);
        var session = await dbContext.RefreshSessions
            .SingleOrDefaultAsync(
                item => item.TokenHash == tokenHash,
                cancellationToken);

        if (session is null || session.RevokedAtUtc is not null)
        {
            return;
        }

        session.RevokedAtUtc = timeProvider.GetUtcNow();
        session.RevokedByIp = Normalize(ipAddress, 64);
        session.RevocationReason = "Logout";

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CurrentUser?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(
                item => item.Id == userId && item.IsActive,
                cancellationToken);

        return user is null
            ? null
            : await BuildCurrentUserAsync(user, cancellationToken);
    }

    private async Task<CurrentUser> BuildCurrentUserAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var roles = (await userManager.GetRolesAsync(user))
            .Order(StringComparer.Ordinal)
            .ToArray();

        var permissions = await dbContext.RolePermissions
            .AsNoTracking()
            .Where(rolePermission =>
                rolePermission.Role.Name != null &&
                roles.Contains(rolePermission.Role.Name) &&
                rolePermission.Permission.IsActive)
            .Select(rolePermission => rolePermission.Permission.Code)
            .Distinct()
            .OrderBy(code => code)
            .ToArrayAsync(cancellationToken);

        return new CurrentUser(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            roles,
            permissions);
    }

    private SessionCreationResult CreateSession(
        ApplicationUser user,
        CurrentUser currentUser,
        DateTimeOffset now,
        string? ipAddress,
        string? userAgent)
    {
        var accessToken = tokenGenerator.CreateAccessToken(
            currentUser,
            now);
        var refreshToken = tokenGenerator.CreateRefreshToken(now);

        var entity = new RefreshSession
        {
            UserId = user.Id,
            TokenHash = refreshToken.TokenHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = refreshToken.ExpiresAtUtc,
            CreatedByIp = Normalize(ipAddress, 64),
            UserAgent = Normalize(userAgent, 512)
        };

        var response = new AuthenticationSession(
            accessToken.Token,
            refreshToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.ExpiresAtUtc,
            currentUser);

        return new SessionCreationResult(entity, response);
    }

    private async Task RevokeActiveSessionsAsync(
        Guid userId,
        DateTimeOffset now,
        string? ipAddress,
        string reason,
        CancellationToken cancellationToken)
    {
        var sessions = await dbContext.RefreshSessions
            .Where(session =>
                session.UserId == userId &&
                session.RevokedAtUtc == null &&
                session.ExpiresAtUtc > now)
            .ToArrayAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.RevokedAtUtc = now;
            session.RevokedByIp = Normalize(ipAddress, 64);
            session.RevocationReason = reason;
        }
    }

    private static string? Normalize(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmedValue = value.Trim();
        return trimmedValue.Length <= maximumLength
            ? trimmedValue
            : trimmedValue[..maximumLength];
    }

    private sealed record SessionCreationResult(
        RefreshSession Entity,
        AuthenticationSession Response);
}
