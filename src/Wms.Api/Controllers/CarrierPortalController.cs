using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Application.Shipping;

namespace Wms.Api.Controllers;

[ApiController, Route("api/v1/carrier-portal/pickups")]
public sealed class CarrierPortalController(IShippingService service) : ControllerBase
{
    [AllowAnonymous, HttpGet("{accessToken:guid}")]
    public async Task<IActionResult> Get(Guid accessToken, CancellationToken cancellationToken)
    {
        var pickup = await service.GetPublicPickupAsync(accessToken, cancellationToken);
        return pickup is null ? NotFound() : Ok(pickup);
    }
}
