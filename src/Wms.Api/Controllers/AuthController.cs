using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Authentication;
using Wms.Application.Authentication;

namespace Wms.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
    IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthenticationSession>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authenticationService.LoginAsync(
            request.Email,
            request.Password,
            GetClientIpAddress(),
            GetUserAgent(),
            cancellationToken);

        if (result.Succeeded)
        {
            return Ok(result.Session);
        }

        if (result.FailureReason == AuthenticationFailureReason.LockedOut)
        {
            return Problem(
                statusCode: StatusCodes.Status429TooManyRequests,
                title: "Conta temporariamente bloqueada",
                detail: "Aguarde alguns minutos antes de tentar novamente.");
        }

        return Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Credenciais inválidas",
            detail: "E-mail ou senha inválidos.");
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthenticationSession>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var session = await authenticationService.RefreshAsync(
            request.RefreshToken,
            GetClientIpAddress(),
            GetUserAgent(),
            cancellationToken);

        return session is null
            ? Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Sessão inválida",
                detail: "O refresh token é inválido, expirou ou foi revogado.")
            : Ok(session);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAsync(
            request.RefreshToken,
            GetClientIpAddress(),
            cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var currentUser = await authenticationService.GetCurrentUserAsync(
            userId,
            cancellationToken);

        return currentUser is null
            ? Unauthorized()
            : Ok(currentUser);
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return Request.Headers.UserAgent.FirstOrDefault();
    }
}
