using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Application.Authorization;
using Wms.Application.Users;

namespace Wms.Api.Controllers;

[ApiController]
[Authorize(Policy = SystemPermissions.Roles.Read)]
[Route("api/v1/roles")]
public sealed class RolesController(
    IUserAdministrationService userAdministrationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<RoleSummary>>(
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var roles = await userAdministrationService.GetRolesAsync(
            cancellationToken);

        return Ok(roles);
    }
}
