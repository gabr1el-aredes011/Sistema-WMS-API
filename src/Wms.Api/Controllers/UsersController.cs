using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Users;
using Wms.Application.Authorization;
using Wms.Application.Users;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public sealed class UsersController(
    IUserAdministrationService userAdministrationService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.Users.Read)]
    [ProducesResponseType<PagedResult<UserSummary>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await userAdministrationService.GetUsersAsync(
            search,
            isActive,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Policy = SystemPermissions.Users.Read)]
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userAdministrationService.GetUserAsync(
            userId,
            cancellationToken);

        return user is null
            ? NotFound()
            : Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.Users.Create)]
    [Authorize(Policy = SystemPermissions.Users.ManageRoles)]
    [ProducesResponseType<UserDetails>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userAdministrationService.CreateUserAsync(
            new CreateUserCommand(
                request.FullName,
                request.Email,
                request.Password,
                request.Roles),
            cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(
                nameof(GetById),
                new { userId = result.Value!.Id },
                result.Value)
            : ToProblem(result);
    }

    [HttpPut("{userId:guid}")]
    [Authorize(Policy = SystemPermissions.Users.Update)]
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userAdministrationService.UpdateUserAsync(
            userId,
            new UpdateUserCommand(request.FullName, request.Email),
            cancellationToken);

        return result.Succeeded
            ? Ok(result.Value)
            : ToProblem(result);
    }

    [HttpPut("{userId:guid}/status")]
    [Authorize(Policy = SystemPermissions.Users.Disable)]
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetStatus(
        Guid userId,
        SetUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        var actorUserId = GetCurrentUserId();

        if (actorUserId is null)
        {
            return Unauthorized();
        }

        var result = await userAdministrationService.SetUserStatusAsync(
            actorUserId.Value,
            userId,
            request.IsActive,
            cancellationToken);

        return result.Succeeded
            ? Ok(result.Value)
            : ToProblem(result);
    }

    [HttpPut("{userId:guid}/roles")]
    [Authorize(Policy = SystemPermissions.Users.ManageRoles)]
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetRoles(
        Guid userId,
        SetUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        var actorUserId = GetCurrentUserId();

        if (actorUserId is null)
        {
            return Unauthorized();
        }

        var result = await userAdministrationService.SetUserRolesAsync(
            actorUserId.Value,
            userId,
            request.Roles,
            cancellationToken);

        return result.Succeeded
            ? Ok(result.Value)
            : ToProblem(result);
    }

    private Guid? GetCurrentUserId()
    {
        return Guid.TryParse(User.FindFirst("sub")?.Value, out var userId)
            ? userId
            : null;
    }

    private IActionResult ToProblem<T>(
        UserAdministrationResult<T> result)
    {
        var statusCode = result.Failure switch
        {
            UserAdministrationFailure.NotFound => StatusCodes.Status404NotFound,
            UserAdministrationFailure.Conflict => StatusCodes.Status409Conflict,
            UserAdministrationFailure.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(
            statusCode: statusCode,
            title: "Não foi possível concluir a operação",
            detail: string.Join(" ", result.Errors));
    }
}
