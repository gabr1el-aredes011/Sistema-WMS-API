using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wms.Api.Contracts.Shipping;
using Wms.Application.Authorization;
using Wms.Application.Shipping;

namespace Wms.Api.Controllers;

[ApiController, Authorize, Route("api/v1/carriers")]
public sealed class CarriersController(IShippingService service) : ControllerBase
{
    [HttpGet, Authorize(Policy = SystemPermissions.Carriers.Read)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken) => Ok(await service.GetCarriersAsync(isActive, cancellationToken));

    [HttpPost, Authorize(Policy = SystemPermissions.Carriers.Manage)]
    public async Task<IActionResult> Create(CreateCarrierRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateCarrierAsync(new(request.Name, request.TaxId, request.ContactName, request.Email, request.Phone), cancellationToken);
        return result.Succeeded ? Created($"/api/v1/carriers/{result.Value!.Id}", result.Value) : Problem(statusCode: 400, title: "Não foi possível cadastrar a transportadora", detail: string.Join(" ", result.Errors));
    }

    [HttpPut("{carrierId:guid}/status"), Authorize(Policy = SystemPermissions.Carriers.Manage)]
    public async Task<IActionResult> SetStatus(Guid carrierId, SetCarrierStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await service.SetCarrierStatusAsync(carrierId, request.IsActive, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Problem(statusCode: 404, title: "Transportadora não encontrada", detail: string.Join(" ", result.Errors));
    }
}
