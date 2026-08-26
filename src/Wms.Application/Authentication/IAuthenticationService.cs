namespace Wms.Application.Authentication;

public interface IAuthenticationService
{
    Task<LoginResult> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<AuthenticationSession?> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<CurrentUser?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
