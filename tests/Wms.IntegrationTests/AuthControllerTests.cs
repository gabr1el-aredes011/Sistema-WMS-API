using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Authentication;
using Wms.Api.Controllers;
using Wms.Application.Authentication;

namespace Wms.IntegrationTests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_InactiveUser_ReturnsSpecificForbiddenProblem()
    {
        var controller = CreateController(AuthenticationFailureReason.InactiveUser);

        var result = await controller.Login(
            new LoginRequest
            {
                Email = "inactive@pvcompany.com.br",
                Password = "irrelevant-password"
            },
            CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
        Assert.Equal("Perfil inativo", problem.Title);
        Assert.Equal(
            "Seu perfil está inativo. Entre em contato com o seu administrador.",
            problem.Detail);
    }

    [Fact]
    public async Task Login_InvalidCredentials_RemainsGenericUnauthorizedProblem()
    {
        var controller = CreateController(AuthenticationFailureReason.InvalidCredentials);

        var result = await controller.Login(
            new LoginRequest
            {
                Email = "unknown@pvcompany.com.br",
                Password = "invalid-password"
            },
            CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);
        Assert.Equal("Credenciais inválidas", problem.Title);
    }

    private static AuthController CreateController(
        AuthenticationFailureReason failureReason)
    {
        return new AuthController(new StubAuthenticationService(failureReason))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private sealed class StubAuthenticationService(
        AuthenticationFailureReason failureReason) : IAuthenticationService
    {
        public Task<LoginResult> LoginAsync(
            string email,
            string password,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new LoginResult(failureReason, null));

        public Task<AuthenticationSession?> RefreshAsync(
            string refreshToken,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<AuthenticationSession?>(null);

        public Task LogoutAsync(
            string refreshToken,
            string? ipAddress,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<CurrentUser?> GetCurrentUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<CurrentUser?>(null);
    }
}
