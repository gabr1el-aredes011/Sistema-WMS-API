using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Shipping;
using Wms.Application.Authorization;
using Wms.Application.Shipping;
using Wms.Domain.Shipping;

namespace Wms.Api.Controllers;

[ApiController, Authorize, Route("api/v1/dispatch/pickups")]
public sealed class DispatchController(IShippingService service) : ControllerBase
{
    [HttpGet, Authorize(Policy = SystemPermissions.Dispatch.Read)]
    public async Task<IActionResult> GetAll([FromQuery] PickupStatus? status, CancellationToken cancellationToken) => Ok(await service.GetPickupsAsync(status, cancellationToken));

    [HttpPost, Authorize(Policy = SystemPermissions.Dispatch.Manage)]
    public async Task<IActionResult> Create(CreatePickupRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreatePickupAsync(new(request.CarrierId, request.OrderReference, request.Description, request.VolumeCount, request.ScheduledAtUtc), cancellationToken);
        return result.Succeeded ? Created($"/api/v1/dispatch/pickups/{result.Value!.Id}", result.Value) : Problem(statusCode: 400, title: "Não foi possível criar a coleta", detail: string.Join(" ", result.Errors));
    }

    [HttpPut("{pickupId:guid}/status"), Authorize(Policy = SystemPermissions.Dispatch.UpdateReadiness)]
    public async Task<IActionResult> SetStatus(Guid pickupId, SetPickupStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await service.SetPickupStatusAsync(pickupId, request.Status, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(statusCode: 400, title: "Não foi possível atualizar a coleta", detail: string.Join(" ", result.Errors));
    }
}
